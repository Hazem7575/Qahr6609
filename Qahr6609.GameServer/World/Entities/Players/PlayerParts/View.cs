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
    public unsafe partial class Player
    {
        #region Surroundings
        public ConcurrentDictionary<uint, IMapObj> KnownObjects = new ConcurrentDictionary<uint, IMapObj>();
        // Scratch set reused by UpdateSurroundings (guarded by itself: Teleport can run on several threads).
        private readonly Dictionary<uint, IMapObj> surroundingsScratch = new Dictionary<uint, IMapObj>();

        // [Performance] SurroundingsSpawns: 0 = UpdateSurroundings only keeps KnownObjects (and despawns) up to date
        // and leaves spawning to RoleView.Role(), which already sends every spawn with the Invisible/watching rules;
        // 1 = old behaviour (it also re-sends both players' spawns for every pair it newly tracks).
        public static readonly bool SurroundingsSpawns = Conquer6609.Framework.Settings.Int("Performance", "SurroundingsSpawns", 0) != 0;

        public void UpdateSurroundings(ServerSockets.Packet stream, bool clear = false, bool dissconnect = false)
        {
            if (dissconnect)
            {
                //Remove me from all nearby entities when logout
                // (runs before the map 700 / watching early-outs so a disconnected watcher is removed too)
                foreach (var entry in KnownObjects)
                {
                    var knownPlayer = entry.Value as Player;
                    if (knownPlayer != null)
                        knownPlayer.TryDespawn(stream, this);
                }
                return;
            }
            if (Map != 0)
            {
                if (Map == 700)
                    return;
            }
            if (Owner.IsWatching())
            {
                return;
            }
            if (clear)
            {
                //Remove me from all nearby entities
                foreach (var entry in KnownObjects)
                {
                    var knownPlayer = entry.Value as Player;
                    if (knownPlayer != null)
                        knownPlayer.TryDespawn(stream, this);
                }

                //Reset my Known Objects collection
                KnownObjects.Clear();
            }

            lock (surroundingsScratch)
            {
                var newObjects = surroundingsScratch;
                newObjects.Clear();
                try
                {
                    foreach (var obj in View.Roles(Role.MapObjectType.Player))
                        newObjects[obj.UID] = obj;

                    //Remove objects that are no longer on screen (same object instance, like the old List.Contains)
                    foreach (var entry in KnownObjects)
                    {
                        IMapObj current;
                        if (!newObjects.TryGetValue(entry.Key, out current) || !ReferenceEquals(current, entry.Value))
                            TryDespawn(stream, entry.Value);
                    }

                    //Add new objects (objects already on screen are skipped)
                    foreach (var entry in newObjects)
                    {
                        if (KnownObjects.ContainsKey(entry.Key))
                            continue;
                        if (SurroundingsSpawns)
                            TryAdd(stream, entry.Value);
                        else
                            TrackKnown(entry.Value);
                    }
                }
                finally
                {
                    newObjects.Clear();
                }
            }
        }

        /// <summary>TryAdd without packets: Role() has already exchanged the spawns of both players.</summary>
        private void TrackKnown(IMapObj t)
        {
            if (!KnownObjects.TryAdd(t.UID, t))
                return;
            var target = t as Player;
            if (target != null)
                target.TrackKnown(this);
        }

        public void TryAdd(ServerSockets.Packet stream, IMapObj t)
        {
            if (!KnownObjects.TryAdd(t.UID, t))
                return;
            var target = t as Player;
            Send(target.GetArray(stream, false));
            target.TryAdd(stream, this);
        }

        public void TryDespawn(ServerSockets.Packet stream, IMapObj t)
        {
            IMapObj x;
            if (!KnownObjects.TryRemove(t.UID, out x))
                return;
            if (t is Player)
            {
                var target = t as Player;
                Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                {
                    ObjId = t.UID,
                    Type = Game.MsgServer.ActionType.RemoveEntity
                };
                Send(stream.ActionCreate(&action));
                target.TryDespawn(stream, this);
            }
        }
        #endregion
        public RoleView View;
        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(Px, Py, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            return ((OldGetDistance(X2, Y2) > distance) && GetMyDistance(X2, Y2) <= distance);
        }

        public void ClearItemsSpawn()
        {
            HeadId = GarmentId = WingId = WingProgress = ArmorId = LeftWeaponId = RightWeaponId = LeftWeaponAccessoryId = RightWeaponAccessoryId = SteedId = MountArmorId = 0;
            ColorArmor = ColorShield = ColorHelment = 0;
            SteedPlus = SteedColor = WingPlus = 0;
            HeadSoul = ArmorSoul = LeftWeapsonSoul = RightWeapsonSoul = 0;
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool WindowsView)
        {
            //  Console.WriteLine(this.GetHashCode());
            stream.InitWriter();

            stream.Write(Extensions.Time32.Now.Value);

            stream.Write(Mesh);//(uint)(TransformationID * 10000000 + Face * 10000 + Body));

            stream.Write(UID);
            stream.Write(GuildID);
            if (Program.ServerConfig.IsInterServer == false && Owner.OnInterServer == false)
                stream.Write((ushort)GuildRank);
            else
                stream.ZeroFill(2);

            stream.Write((uint)0);//unknow


            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write(BitVector.bits[x]);


            if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.FreezeWar
                || Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.FootBall
                || Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.TeamDeathMatch)
            {
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                    stream.Write((ushort)0);
                else
                    stream.Write((ushort)AparenceType);
            }
            else
                stream.Write((ushort)AparenceType);//apparence type
            stream.Write(HeadId);
            stream.Write(GarmentId);
            stream.Write(ArmorId);
            stream.Write(LeftWeaponId);
            stream.Write(RightWeaponId);
            stream.Write(LeftWeaponAccessoryId);
            stream.Write(RightWeaponAccessoryId);
            stream.Write(SteedId);
            stream.Write(MountArmorId);
            stream.Write(WingId);
            stream.Write((byte)WingPlus);//talisman plus
            stream.Write(WingProgress);

            stream.Write((uint)0);//?? unknow

            stream.ZeroFill(6);//unknow

            stream.Write(HitPoints);
            stream.Write((ushort)0);//unknow
            stream.Write((ushort)0);//monster level

            stream.Write(X);
            stream.Write(Y);
            stream.Write(Hair);
            stream.Write((byte)Angle);
            stream.Write((uint)Action);
            stream.Write((ushort)0);//unknow
            stream.Write((byte)0);//padding?
            stream.Write(Reborn);
            stream.Write(Level);


            stream.Write((byte)(WindowsView ? 1 : 0));
            stream.Write((byte)Away);//away
            stream.Write(ExtraBattlePower);
            stream.Write((uint)0);//unknow position = 125
            stream.Write((uint)0);//unknow position = 129
            stream.Write((uint)0);//unknow p = 133;
            stream.Write((uint)(FlowerRank + 10000));
            stream.Write((uint)NobilityRank);

            stream.Write(ColorArmor);
            stream.Write(ColorShield);
            stream.Write(ColorHelment);
            stream.Write((uint)0);//quiz points
            stream.Write(SteedPlus);
            stream.Write((ushort)0);//unknow
            stream.Write(SteedColor);
            stream.Write((ushort)Enilghten);
            stream.Write((ushort)0);//merit points
            stream.Write((uint)0);//unknow
            stream.Write((uint)0);//unknow

            stream.Write(ClanUID);
            stream.Write((uint)ClanRank);





            stream.Write((uint)0);//unknow
            stream.Write((ushort)MyTitle);

            stream.Write((uint)PokerTableID);
            stream.Write((uint)PokerSeat);
            stream.Write((byte)0);

            stream.Write((byte)(Owner.IsWatching() ? 1 : 0));//1 == invisible player watcher
            stream.Write((byte)0);
            stream.Write((byte)0);
            stream.Write((byte)0);

            // stream.ZeroFill(14);
            stream.Write(HeadSoul);
            stream.Write(ArmorSoul);
            stream.Write(LeftWeapsonSoul);
            stream.Write(RightWeapsonSoul);
            stream.Write((byte)ActiveSublass);
            stream.Write(SubClassHasPoints);
            stream.Write((uint)0);//unknow
            stream.Write((ushort)FirstClass);
            stream.Write((ushort)SecoundeClass);
            stream.Write((ushort)Class);

            stream.Write((ushort)CountryID);//country
            if (Owner.Team != null)
            {
                stream.Write((uint)Owner.Team.UID);
            }
            else
                stream.Write(0);



            stream.Write(BattlePower);
            stream.Write(JiangHuTalent);
            stream.Write(JiangHuActive);

            //      stream.Write((ushort)2);
            stream.Write((byte)0);
            if (OnMyOwnServer == false)
                stream.Write(ServerID);
            else
                stream.ZeroFill(2);
            stream.Write((uint)RealUID);
            stream.Write((byte)2);//clone count 
            stream.Write((ushort)0); // clone ID
            stream.Write(0); //clone owner

            if (InUnion)
            {
                stream.Write((uint)MyUnion.UID);
                stream.Write((uint)ExploitsRank);//??
                stream.Write((uint)Role.Instance.Union.Member.GetRank(UnionMemeber.Rank));//UnionMemeber.Rank);        
                stream.Write((byte)(UnionMemeber.Rank == Instance.Union.Member.MilitaryRanks.Emperor ? 1 : 0));
                stream.Write((byte)MyUnion.IsKingdom);
            }
            else
            {
                stream.ZeroFill(4);
                stream.Write((uint)ExploitsRank);
                stream.ZeroFill(6);
            }
            stream.Write(SpecialTitleID);
            stream.Write(SpecialTitleScore);
            stream.Write(SpecialWingID);

            stream.Write((uint)MainFlag);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write((uint)0);
            if (OnMyOwnServer == false)
            {
                if (InUnion)
                    stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, MyGuild != null ? MyGuild.GuildName : string.Empty, MyUnion.Name);
                else
                    stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, MyGuild != null ? MyGuild.GuildName : string.Empty, string.Empty);
            }
            else
            {
                if (InUnion)
                    stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, string.Empty, MyUnion.Name);
                else
                    stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, string.Empty, string.Empty);
            }
            stream.Finalize(Game.GamePackets.SpawnPlayer);

            return stream;

        }
    }

}
