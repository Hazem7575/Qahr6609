using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Conquer6609.Game.MsgFloorItem;
using System.IO;
using Conquer6609.ServerSockets;
using ProtoBuf;
using Conquer6609.Database;
using System.Collections.Concurrent;
namespace Conquer6609.Game.MsgServer
{
    [ProtoContract]
    public class WalkQuery
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Direction;
        [ProtoMember(2, IsRequired = true)]
        public uint UID;
        [ProtoMember(3, IsRequired = true)]
        public uint Running;
        [ProtoMember(4, IsRequired = true)]
        public uint TimeStamp;
    }
    public static unsafe class MsgMovement
    {
        public const uint Walk = 0, Run = 1, Steed = 9;


        public static sbyte[] DeltaMountX = new sbyte[24] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] DeltaMountY = new sbyte[24] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };

        
        public static unsafe void GetWalk(this ServerSockets.Packet stream, out WalkQuery pQuery)
        {
          pQuery = new WalkQuery();
          pQuery=  stream.ProtoBufferDeserialize<WalkQuery>(pQuery);
        }

        public static unsafe ServerSockets.Packet MovementCreate(this ServerSockets.Packet stream, WalkQuery pQuery)
        {
            stream.InitWriter();
            pQuery.TimeStamp = (uint)Extensions.Time32.Now.Value;
            stream.ProtoBufferSerialize(pQuery);
        
            stream.Finalize(GamePackets.Movement);

            return stream;
        }

        public static uint Bodyyyy = 0;
        public static uint UIDDDD = 1000000;
        public static int eeffect = 1;
        public static int LastClientStamp = 0;

        // Movement rate rule: per-player token bucket. Up to StepBurst steps may arrive at once
        // (network bunching); sustained rate is one step per StepRefillMs (half that while riding
        // or under a speed effect / speed transformation). Excess steps are rejected and the player
        // is pulled back to the server-known position.
        public static int StepBurst = 10;
        public static int StepRefillMs = 80;
        class StepBucket { public double Tokens; public DateTime Last; }
        static readonly ConcurrentDictionary<uint, StepBucket> StepBuckets = new ConcurrentDictionary<uint, StepBucket>();
        static readonly bool stepBucketsForgetHook = Conquer6609.Security.Guard.RegisterForgetHook(Forget);
        /// <summary>Drops the player's step bucket (called on logout via Guard.ForgetPlayer).</summary>
        public static void Forget(uint uid)
        {
            StepBucket b;
            StepBuckets.TryRemove(uid, out b);
        }

        static bool AllowStep(Client.GameClient client)
        {
            var now = DateTime.UtcNow;
            StepBucket bucket;
            if (!StepBuckets.TryGetValue(client.Player.UID, out bucket))
                bucket = StepBuckets.GetOrAdd(client.Player.UID, new StepBucket() { Tokens = StepBurst, Last = now });
            bool fast = client.Player.OnSpeedFlags() || client.Player.OnTransform
                || client.Player.ContainFlag(MsgUpdate.Flags.Ride) || client.Player.ContainFlag(MsgUpdate.Flags.Fly)
                || client.Player.TransformationID == 207 || client.Player.TransformationID == 267;
            double refill = fast ? StepRefillMs / 2.0 : StepRefillMs;
            lock (bucket)
            {
                double elapsed = (now - bucket.Last).TotalMilliseconds;
                if (elapsed < 0) elapsed = 0;
                bucket.Tokens = Math.Min(StepBurst, bucket.Tokens + elapsed / refill);
                bucket.Last = now;
                if (bucket.Tokens < 1)
                    return false;
                bucket.Tokens -= 1;
                return true;
            }
        }

        static void RejectStep(Client.GameClient client, string reason, int x, int y)
        {
            if (Conquer6609.Security.Guard.Throttle("walk-reject:" + client.Player.UID, 2000))
                Conquer6609.Security.Guard.Audit("walk-reject", client.Player.UID, client.Player.Name,
                    reason + " map=" + client.Player.Map + " from=" + client.Player.X + "," + client.Player.Y + " to=" + x + "," + y);
            client.SoftPullback();
        }
        [PacketAttribute(GamePackets.Movement)]
        public unsafe static void Movement(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.AutoHunting.Enable)
            {//ayman
                client.SoftPullback();
                client.Player.MessageBoxx("Can`t move while auto hunting, do you want to stop auto hunting?", new Action<Client.GameClient>(p =>
                {
                    Catching.End(client);
                }), null, 0, MsgStaticMessage.Messages.None, true);
                return;
            }
            Client.GameClient user = null;
            if (Server.GamePoll.TryGetValue(client.Player.UID, out user))
            {
                client.CheckPendingPullback();
                if (client.Player.Away == 1)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var apacket = rec.GetStream();
                        client.Player.Away = 0;
                        client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                    }
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    client.Player.UpdateSurroundings(stream);
                }
                if (client.Player.Mining)
                {
                    client.Player.StopMining();
                }
                Role.Flags.ConquerAngle dir;

                WalkQuery walkPacket;

                packet.GetWalk(out walkPacket);
                walkPacket.UID = client.Player.UID;


                client.Player.Action = Role.Flags.ConquerAction.None;

                client.OnAutoAttack = false;
                client.Player.RemoveBuffersMovements(packet);

                client.Player.Protect = Extensions.Time32.Now;
                client.Player.LastMove = DateTime.Now;

                if (client.Map == null)
                {
                    client.Teleport(310, 288, 1002);
                    return;
                }
                if (!AllowStep(client))
                {
                    RejectStep(client, "rate", client.Player.X, client.Player.Y);
                    return;
                }
                if (walkPacket.Running == MsgMovement.Steed)
                {

                    dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 24);

                    int newX = client.Player.X + DeltaMountX[(byte)dir];
                    int newY = client.Player.Y + DeltaMountY[(byte)dir];
                    if (client.Map == null)
                    {
                        client.Teleport(310, 288, 1002);
                        return;
                    }
                    // Steed steps (2 tiles) are only valid while mounted, and must land inside the map on
                    // walkable DMap terrain (NPC footprints are ignored, see GameMap.ValidTerrain).
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.Ride))
                    {
                        RejectStep(client, "steed step without ride", newX, newY);
                        return;
                    }
                    if (!client.Map.ValidTerrain(newX, newY))
                    {
                        RejectStep(client, "invalid tile", newX, newY);
                        return;
                    }
                    if (client.Player.Map == 1038)
                    {
                        // ValidWalk ignores its MovingLine argument: don't build one per step.
                        if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, (ushort)newX, (ushort)newY, null))
                        {
                            client.SendSysMesage("Illegal jumping over the gates detected.");

                            client.SoftPullback();
                            return;
                        }
                    }
                    if (!Conquer6609.Framework.Modules.ModuleManager.AllowMove(client, (ushort)newX, (ushort)newY)) return;
                    // Serialize the movement once: Send copies the bytes, nothing writes to `packet` in between.
                    var steedMove = packet.MovementCreate(walkPacket);
                    client.Player.View.SendView(steedMove, true);
                    client.Map.View.MoveTo<Role.IMapObj>(client.Player, newX, newY);
                    client.Player.X = (ushort)newX;
                    client.Player.Y = (ushort)newY;

                    client.Player.Action = Role.Flags.ConquerAction.None;
                    client.Player.View.Role(false, steedMove);

                    if (client.Vigor >= 2)
                        client.Vigor -= 2;
                    else
                        client.Vigor = 0;

                    client.Send(packet.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));


                }
                else
                {
                    ushort walkX = client.Player.X, walkY = client.Player.Y;
                    dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 8);
                    Role.Core.IncXY(dir, ref walkX, ref walkY);


                    if (client.Map == null)
                    {
                        client.Teleport(310, 288, 1002);
                        return;
                    }
                    if (!client.Map.ValidTerrain(walkX, walkY))
                    {
                        RejectStep(client, "invalid tile", walkX, walkY);
                        return;
                    }
                    if (client.Player.Map == 1038)
                    {
                        // ValidWalk ignores its MovingLine argument: don't build one per step.
                        if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, walkX, walkY, null))
                        {
                            client.SendSysMesage("Illegal jumping over the gates detected.");
                            client.SoftPullback();
                            return;
                        }
                    }
                    if (!Conquer6609.Framework.Modules.ModuleManager.AllowMove(client, walkX, walkY)) return;
                    #region BlueMouse Teleporter
                    if (client.Player.Map == 1025 && client.Player.X >= 166 && client.Player.X <= 169
                        && client.Player.Y >= 110 && client.Player.Y <= 114)
                    {
                        client.Teleport(16, 80, 1502);
                    }
                    if (client.Player.Map == 1025 && client.Player.X >= 112 && client.Player.X <= 117
                      && client.Player.Y >= 159 && client.Player.Y <= 163)
                    {
                        client.Teleport(74, 17, 1503);
                    }
                    if (client.Player.Map == 1502 && client.Player.X >= 10 && client.Player.X <= 14
                      && client.Player.Y >= 77 && client.Player.Y <= 79)
                    {
                        client.Teleport(162, 111, 1025);
                    }
                    if (client.Player.Map == 1503 && client.Player.X >= 72 && client.Player.X <= 74
                     && client.Player.Y >= 11 && client.Player.Y <= 14)
                    {
                        client.Teleport(114, 157, 1025);
                    }
                    #endregion
                    if (client.Player.ObjInteraction != null)
                    {
                        if (client.Player.ObjInteraction.Player.X == client.Player.X && client.Player.ObjInteraction.Player.Y == client.Player.Y)
                        {

                            InterActionWalk query = new InterActionWalk()
                            {
                                Mode = MsgInterAction.Action.Walk,
                                UID = client.Player.UID,
                                OponentUID = client.Player.ObjInteraction.Player.UID,
                                DirectionOne = (byte)dir
                            };

                            client.Player.View.SendView(packet.InterActionWalk(&query), true);

                            client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                            client.Player.X = walkX;
                            client.Player.Y = walkY;
                            client.Player.Angle = dir;

                            client.Player.View.Role(false, packet.InterActionWalk(&query));

                            client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, walkX, walkY);
                            client.Player.ObjInteraction.Player.X = walkX;
                            client.Player.ObjInteraction.Player.Y = walkY;
                            client.Player.ObjInteraction.Player.Angle = dir;

                            client.Player.ObjInteraction.Player.View.Role();
                            return;
                        }
                    }
                    // Serialize the movement once: Send copies the bytes, nothing writes to `packet` in between.
                    var walkMove = packet.MovementCreate(walkPacket);
                    client.Player.View.SendView(walkMove, true);
                    client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                    client.Player.X = walkX;
                    client.Player.Y = walkY;
                    client.Player.Angle = dir;

                    client.Player.View.Role(false, walkMove);
                }
                if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                    MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);
                if (MsgTournaments.MsgSchedules.SteedRace.IsOn)
                {
                    if (MsgTournaments.MsgSteedRace.MAPID == client.Player.Map)
                        MsgTournaments.MsgSchedules.SteedRace.CheckForRaceItems(client);
                }
                if (client.Player.ActivePick)
                    client.Player.RemovePick(packet);



            }



        }
    }
}
