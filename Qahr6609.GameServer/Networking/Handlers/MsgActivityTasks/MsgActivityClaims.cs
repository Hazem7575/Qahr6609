using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetActivityClaims(this ServerSockets.Packet stream, out uint IDClaim)
        {
            IDClaim = stream.ReadUInt8();
        }

        /// <summary>
        /// MsgActivityClaims (2822) server -> client: byte = number of chests claimed today.
        /// The client ignores 0 (use ActivityRewardsStateCreate to reset it).
        /// </summary>
        public static unsafe ServerSockets.Packet ActivityClaimsCreate(this ServerSockets.Packet stream, uint IDClaim)
        {
            stream.InitWriter();

            stream.Write((byte)IDClaim);

            stream.Finalize(GamePackets.MsgActivityClaims);
            return stream;
        }

        /// <summary>
        /// MsgActivityRewardsInfo (2823): byte count, then count x {uint type, uint subtype, uint value}.
        /// type 2 / sub 1 = today's activity points, type 2 / sub 2 = chests claimed today
        /// (client: next claimable chest = value + 1).
        /// </summary>
        public static unsafe ServerSockets.Packet ActivityRewardsStateCreate(this ServerSockets.Packet stream, uint ActivityPoints, uint ClaimedChests)
        {
            stream.InitWriter();

            stream.Write((byte)2);

            stream.Write((uint)2);
            stream.Write((uint)1);
            stream.Write(ActivityPoints);

            stream.Write((uint)2);
            stream.Write((uint)2);
            stream.Write(ClaimedChests);

            stream.Finalize(GamePackets.MsgActivityRewardsInfo);
            return stream;
        }
    }
    public unsafe struct MsgActivityClaims
    {
        // Official chests from the client's ini/activity_reward_type.dat: 3 chests per reborn tier,
        // 100 / 210 / 340 Activity points, claimed in order once per day.
        //   tier 0 and 1: 3005061, 3005064, 3005064    tier 2: 3005062, 3005065, 3005065
        private static readonly uint[][] TierChestItems =
        {
            new uint[] { 3005061, 3005064, 3005064 },
            new uint[] { 3005061, 3005064, 3005064 },
            new uint[] { 3005062, 3005065, 3005065 },
        };
        private static readonly int[] DefaultNeed = { 100, 210, 340 };
        public const int ChestCount = 3;

        public static uint ChestItem(int chest, byte reborn)
        {
            return TierChestItems[Math.Min((int)reborn, 2)][chest - 1];
        }

        // shell.ini [Activeness] Chest1..Chest3 (or Chest<n>Reborn<r>) can override the official values.
        public static uint NeedPoints(int chest, byte reborn)
        {
            if (chest < 1 || chest > DefaultNeed.Length)
                return uint.MaxValue;
            int def = Framework.Settings.Int("Activeness", "Chest" + chest, DefaultNeed[chest - 1]);
            int value = Framework.Settings.Int("Activeness", "Chest" + chest + "Reborn" + Math.Min((int)reborn, 2), def);
            return (uint)Math.Max(0, value);
        }

        [PacketAttribute(GamePackets.MsgActivityClaims)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint IDClaim;

            stream.GetActivityClaims(out IDClaim);

            if (IDClaim >= 1 && IDClaim <= ChestCount)
            {
                if (user.Activeness.ClaimRewards >= IDClaim)
                {
                    user.SendSysMesage("You have already claimed this reward today.");
                }
                else if (user.Activeness.ClaimRewards != IDClaim - 1)
                {
                    user.SendSysMesage("Please claim the previous Activity reward first.");
                }
                else
                {
                    uint need = NeedPoints((int)IDClaim, user.Player.Reborn);
                    if (user.Activeness.ActivityPoints < need)
                    {
                        user.CreateBoxDialog("Should have " + need + " Activity points to claim it.");
                    }
                    else if (!user.Inventory.HaveSpace(1))
                    {
                        user.CreateBoxDialog("Please make 1 more space in your inventory.");
                    }
                    else
                    {
                        user.Activeness.ClaimRewards = (byte)IDClaim;
                        user.Inventory.Add(stream, ChestItem((int)IDClaim, user.Player.Reborn));
                        // success: the client marks the chest as claimed from the claimed count
                        user.Send(stream.ActivityClaimsCreate(user.Activeness.ClaimRewards));
                    }
                }
            }
            // always resync points + real claimed count, so a refused claim is not shown as claimed
            user.Activeness.UpdateActivityPoints(stream);
        }
    }
}
