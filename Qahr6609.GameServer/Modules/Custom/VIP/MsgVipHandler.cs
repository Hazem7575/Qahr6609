using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetVipHandler(this ServerSockets.Packet stream, out MsgVipHandler.VIPTeleportTypes TeleportType, out MsgVipHandler.VIPTeleportLocations Locations,out uint Countdown
            , out string name)
        {
            TeleportType = (MsgVipHandler.VIPTeleportTypes)stream.ReadUInt32();
            Locations = (MsgVipHandler.VIPTeleportLocations)stream.ReadUInt32();
            Countdown = stream.ReadUInt32();

            byte size = stream.ReadUInt8();
            name = stream.ReadCString(size);
        }
        public static unsafe ServerSockets.Packet VipHandlerCreate(this ServerSockets.Packet stream,  MsgVipHandler.VIPTeleportTypes TeleportType,  MsgVipHandler.VIPTeleportLocations Locations,  uint Countdown
         ,  string name)
        {

            stream.InitWriter();

            stream.Write((uint)TeleportType);
            stream.Write((uint)Locations);
            stream.Write(Countdown);

            stream.Write(name.Length);
            stream.Write(name, name.Length);

            stream.Finalize(Game.GamePackets.MsgVipHandler);

            return stream;
        }
    }
    public unsafe struct  MsgVipHandler
    {
        public enum VIPTeleportTypes : uint
        {
            SelfTeleport = 0,
            TeamTeleport = 1,
            TeammateConfirmation = 2,
            TeammateTeleport = 3
        }
        // Ids = first column of the client's ini/VipTrans.ini (the id the client sends).
        public enum VIPTeleportLocations : uint
        {
            TwinCity = 1,
            PhoenixCastle = 2,
            ApeCity = 3,
            DesertCity = 4,
            BirdIland = 5,
            TCSquare = 6,
            WPAltar = 7,
            WPApparation = 8,
            WPPoltergiest = 9,
            WPTurtledove = 10,
            PCSqaure = 11,
            MFWaterCave = 12,
            MFVillage = 13,
            MFLake = 14,
            MFMineCave = 15,
            MFBridge = 16,
            MFToApeCity = 17,
            ACSquare = 18,
            ACSouth = 19,
            ACEast = 20,
            ACNorth = 21,
            ACWest = 22,
            BISquare = 23,
            BINorth = 24,
            BIWest = 25,
            BIEast = 26,
            BIIslandVillage = 27,
            DCSquare = 28,
            DCSouth = 29,
            DCMoonSpring = 30,
            DCAncientMaze = 31
        }

        // VIP level required for the VIP teleport features: [Security] VipTeleportMinLevel in shell.ini
        // (default 1 = any active VIP, own or shared).
        public static int MinVipTeleportLevel
        {
            get
            {
                Conquer6609.Security.Guard.EnsureLoaded();
                return Conquer6609.Security.Guard.VipTeleportMinLevel;
            }
        }
        // Seconds a team-teleport invitation stays valid for the teammates.
        public const int InviteValidSeconds = 30;
        // Minimum interval between two VIP teleport packets of the same player.
        public const int PacketIntervalMs = 1000;

        private class PendingInvite
        {
            public uint InviterUID;
            public VIPTeleportLocations Location;
            public DateTime Expires;
        }
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, PendingInvite> PendingInvites = new System.Collections.Concurrent.ConcurrentDictionary<uint, PendingInvite>();

        private static bool IsStaff(Client.GameClient user)
        {
            return user.GameMaster || user.ProjectManager;
        }

        private static bool IsJailMap(uint map)
        {
            return map == 6000 || map == 6001 || map == 6003;
        }

        // A player may only teleport from a map that allows teleports, outside jail, while alive.
        private static bool CanLeaveMap(Client.GameClient user)
        {
            if (user == null || user.Player == null) return false;
            if (IsJailMap(user.Player.Map)) return false;
            if (Program.BlockTeleportMap.Contains(user.Player.Map)) return false;
            if (!user.Player.Alive) return false;
            return true;
        }

        // VIP teleports require an active VIP of the required level. Player.VipLevel already returns the
        // level of an active shared VIP; an own VIP must not have expired.
        private static bool HasVipTeleport(Client.GameClient user)
        {
            if (user.Player.VipLevel < MinVipTeleportLevel)
                return false;
            return user.Player.ExpireVip > DateTime.Now || HasActiveSharedVip(user.Player.UID);
        }

        private static bool HasActiveSharedVip(uint uid)
        {
            var now = DateTime.Now;
            foreach (var entry in Database.ShareVIP.SharedPoll.GetValues())
            {
                if (entry != null && entry.ShareUID == uid && entry.ShareEnds > now)
                    return true;
            }
            return false;
        }

        private static bool ValidLocation(VIPTeleportLocations location)
        {
            return location >= VIPTeleportLocations.TwinCity && location <= VIPTeleportLocations.DCAncientMaze;
        }

        [PacketAttribute(GamePackets.MsgVipHandler)]
        public unsafe static void Handler(Client.GameClient user, ServerSockets.Packet packet)
        {

                VIPTeleportTypes TeleportType;
         VIPTeleportLocations Locations;
         uint Countdown;
         string name;
     

         packet.GetVipHandler(out TeleportType, out Locations, out Countdown, out name);

            if (!Conquer6609.Security.Guard.Throttle("viptp:" + user.Player.UID, PacketIntervalMs))
                return;
            bool staff = IsStaff(user);
            if (!staff && !CanLeaveMap(user))
            {
                user.SendSysMesage("You can't use the VIP Teleport here.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }

            switch (TeleportType)
            {
                case VIPTeleportTypes.SelfTeleport:
                    {
                        if (!ValidLocation(Locations))
                            break;
                        if (staff)
                        {
                            Teleport(user, Locations);
                        }
                        else if (!HasVipTeleport(user))
                        {
                            user.SendSysMesage("The VIP Teleport requires an active VIP " + MinVipTeleportLevel + ".", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                        }
                        else
                        if (Extensions.Time32.Now > user.LastVIPTeleport.AddMinutes(3))
                        {
                            Teleport(user, Locations);
                            user.LastVIPTeleport = Extensions.Time32.Now;
                        }
                        else
                        {

                            user.SendSysMesage("You have to wait " + (user.LastVIPTeleport.AddMinutes(3) - Extensions.Time32.Now).AllSeconds.ToString() + " more seconds to use the VIP Teleport.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                    
                        }
                        break;
                    }
                case VIPTeleportTypes.TeamTeleport:
                    {
                        if (!ValidLocation(Locations))
                            break;
                        if (!staff && !HasVipTeleport(user))
                        {
                            user.SendSysMesage("The VIP Teleport requires an active VIP " + MinVipTeleportLevel + ".", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            break;
                        }
                        if (staff || Extensions.Time32.Now > user.LastVIPTeamTeleport.AddMinutes(3))
                        {
                            if (user.Team != null)
                            {
                                packet = packet.VipHandlerCreate(VIPTeleportTypes.TeammateConfirmation, Locations, 15, user.Player.Name);

                                foreach (var member in user.Team.Temates)
                                {
                                    if (member.client.Player.UID != user.Player.UID)
                                    {
                                        // Invitations only reach teammates on the same map as the inviter
                                        // ([Security] VipTeamTeleportSameMap, default 1).
                                        if (Conquer6609.Security.Guard.VipTeamTeleportSameMap != 0 && member.client.Player.Map != user.Player.Map)
                                            continue;
                                        PendingInvites[member.client.Player.UID] = new PendingInvite()
                                        {
                                            InviterUID = user.Player.UID,
                                            Location = Locations,
                                            Expires = DateTime.Now.AddSeconds(InviteValidSeconds)
                                        };
                                        member.client.Send(packet);
                                    }
                                }
                            }
                            Teleport(user, Locations);
                            user.LastVIPTeamTeleport = Extensions.Time32.Now;
                        }
                        else
                        {
                            user.SendSysMesage("You have to wait " + (user.LastVIPTeamTeleport.AddMinutes(3) - Extensions.Time32.Now).AllSeconds.ToString() + " more seconds to use the VIP Teleport.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                     
                       }
                        break;
                    }
                case VIPTeleportTypes.TeammateTeleport:
                    {
                        // Accepting requires a live invitation from a current teammate; the destination is the invited one.
                        PendingInvite invite;
                        if (!PendingInvites.TryRemove(user.Player.UID, out invite))
                            break;
                        if (invite.Expires < DateTime.Now)
                            break;
                        if (user.Team == null || !user.Team.Members.ContainsKey(invite.InviterUID))
                            break;
                        Teleport(user, invite.Location);
                        break;
                    }
            }
        }

        public static void Teleport(Client.GameClient user, VIPTeleportLocations Location)
        {
            ushort x, y;
            uint map;
            switch (Location)
            {
                case VIPTeleportLocations.TwinCity:
                case VIPTeleportLocations.TCSquare: x = 301; y = 279; map = 1002; break;
                case VIPTeleportLocations.WPAltar: x = 564; y = 793; map = 1002; break;
                case VIPTeleportLocations.WPApparation: x = 340; y = 650; map = 1002; break;
                case VIPTeleportLocations.WPPoltergiest: x = 230; y = 388; map = 1002; break;
                case VIPTeleportLocations.WPTurtledove: x = 627; y = 474; map = 1002; break;

                case VIPTeleportLocations.PhoenixCastle:
                case VIPTeleportLocations.PCSqaure: x = 188; y = 264; map = 1011; break;
                case VIPTeleportLocations.MFWaterCave: x = 380; y = 31; map = 1011; break;
                case VIPTeleportLocations.MFVillage: x = 785; y = 472; map = 1011; break;
                case VIPTeleportLocations.MFLake: x = 369; y = 568; map = 1011; break;
                case VIPTeleportLocations.MFMineCave: x = 924; y = 560; map = 1011; break;
                case VIPTeleportLocations.MFBridge: x = 648; y = 567; map = 1011; break;
                case VIPTeleportLocations.MFToApeCity: x = 475; y = 841; map = 1011; break;

                case VIPTeleportLocations.ApeCity:
                case VIPTeleportLocations.ACSquare: x = 565; y = 562; map = 1020; break;
                case VIPTeleportLocations.ACSouth: x = 699; y = 640; map = 1020; break;
                case VIPTeleportLocations.ACEast: x = 624; y = 337; map = 1020; break;
                case VIPTeleportLocations.ACNorth: x = 200; y = 224; map = 1020; break;
                case VIPTeleportLocations.ACWest: x = 322; y = 621; map = 1020; break;

                case VIPTeleportLocations.BirdIland:
                case VIPTeleportLocations.BISquare: x = 717; y = 571; map = 1015; break;
                // Bird Island North / West / East / Island Village: approximate spots (no official coordinates
                // in the client data); SafeSpot below moves them to the nearest walkable cell.
                case VIPTeleportLocations.BINorth: x = 125; y = 323; map = 1015; break;
                case VIPTeleportLocations.BIWest: x = 445; y = 720; map = 1015; break;
                case VIPTeleportLocations.BIEast: x = 625; y = 425; map = 1015; break;
                case VIPTeleportLocations.BIIslandVillage: x = 340; y = 395; map = 1015; break;

                case VIPTeleportLocations.DesertCity:
                case VIPTeleportLocations.DCSquare: x = 500; y = 650; map = 1000; break;
                case VIPTeleportLocations.DCSouth: x = 758; y = 750; map = 1000; break;
                case VIPTeleportLocations.DCMoonSpring: x = 291; y = 450; map = 1000; break;
                case VIPTeleportLocations.DCAncientMaze: x = 87; y = 321; map = 1000; break;
                default: return;
            }
            SafeSpot(map, ref x, ref y);
            user.Teleport(x, y, map);
        }

        /// <summary>Moves a destination that is not walkable to the nearest walkable cell (up to 20 cells away).</summary>
        static void SafeSpot(uint mapId, ref ushort x, ref ushort y)
        {
            Role.GameMap map;
            if (Database.Server.ServerMaps == null || !Database.Server.ServerMaps.TryGetValue(mapId, out map) || map == null)
                return;
            if (map.ValidLocation(x, y))
                return;
            for (int r = 1; r <= 20; r++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    for (int dy = -r; dy <= r; dy++)
                    {
                        if (Math.Abs(dx) != r && Math.Abs(dy) != r)
                            continue;
                        int nx = x + dx, ny = y + dy;
                        if (nx < 0 || ny < 0 || nx > ushort.MaxValue || ny > ushort.MaxValue)
                            continue;
                        if (map.ValidLocation((ushort)nx, (ushort)ny))
                        {
                            x = (ushort)nx;
                            y = (ushort)ny;
                            return;
                        }
                    }
                }
            }
        }


    }
}
