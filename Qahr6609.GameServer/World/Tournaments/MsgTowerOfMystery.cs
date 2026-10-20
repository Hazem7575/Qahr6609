using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Game.MsgTournaments
{
    public class MsgTowerOfMystery
    {
        public enum RewardTypes : byte
        {
            Normal = 0,
            Refined = 1,
            Unique = 2,
            Elite = 3,
            Super = 5,
            Count = 6

        }
        //200020,2,40460,4000,66,66


        //new_gamebegin <-- effect
        /*
40930 500046,906,GreatPheasant(Nor.),68,68,4000 layer 1

layer 2
{
40931 500052,397,WingedLord(Nor.),68,68,4000
40932 500028,210,LordProtector(Nor.),75,63,4000
}

40933 layer 3 -> 500056,273,WaterTerror(Nor.),68,68,4000

40934 layer 4 500038,953,RuthlessAsura(Nor.),68,68,4003

40935 layer 5 500060,251,SoulStrangler(Nor.),68,68,4003 lavabeast

40936 500042,952,DarkGlutton(Nor.),68,68,4003 layer 6

40937 500054,950,AlienDragon(Nor.),68,68,4006 layer 7
40938 500022,951,ShadowSpider(Nor.),68,68,4006 layer 8
40939 500084,924,FlameGiant(Nor.),68,68,4008 layer 9*/

        public Game.MsgMonster.MonsterFamily GetMonster(Client.GameClient user)
        {
            Game.MsgMonster.MonsterFamily monster = null;

            switch (user.Player.JoinTowerOfMysteryLayer)
            {
                case 0:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40930 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 1:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40931 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 2:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40933 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 3:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40934 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 4:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40935 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 5:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40936 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 6:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40937 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 7:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40938 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                default:
                    Database.Server.MonsterFamilies.TryGetValue((uint)(40939 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
            }
            return monster;
        }
        public void GenerateReward(Client.GameClient user)
        {
            user.Player.TOMRefreshReward += 1;
            if (Role.Core.Rate(10, 100))
            {
                user.Player.TOM_Reward = RewardTypes.Super;
            }
            else if (Role.Core.Rate(20, 100))
                user.Player.TOM_Reward = RewardTypes.Elite;
            else if (Role.Core.Rate(30, 100))
                user.Player.TOM_Reward = RewardTypes.Unique;
            else if (Role.Core.Rate(40, 100))
                user.Player.TOM_Reward = RewardTypes.Refined;
            else
                user.Player.TOM_Reward = RewardTypes.Normal;

        }

        // Minimum level for teammates brought along into the tower.
        public static int MinTeamMemberLevel = 90;
        public const byte MaxLayer = 8;

        private static bool IsJailMap(uint map)
        {
            return map == 6000 || map == 6001 || map == 6003;
        }

        /// <summary>Common floors require the previous floor cleared; Elite floors require Elite mode unlocked
        /// (all common floors cleared) and the previous Elite floor cleared.</summary>
        public static bool CanEnterLayer(Client.GameClient user, int layer)
        {
            if (layer < 0 || layer > MaxLayer)
                return false;
            if (user.Player.TOMChallengeToday == 1)
            {
                if (user.Player.MyTowerOfMysteryLayer < 9)
                    return false;
                return layer <= Math.Min((int)MaxLayer, (int)user.Player.MyTowerOfMysteryLayerElite);
            }
            return layer <= Math.Min((int)MaxLayer, (int)user.Player.MyTowerOfMysteryLayer);
        }

        /// <summary>Teammates are only brought along when they stand next to the leader on the leader's map,
        /// are alive, meet the level requirement and are not in jail.</summary>
        private static void PullTeam(Client.GameClient user, ushort x, ushort y, uint mapId, uint dyn, uint leaderMap, uint leaderDyn, ushort leaderX, ushort leaderY)
        {
            if (user.Team == null)
                return;
            var rnd = new Random();
            foreach (var members in user.Team.Temates)
            {
                if (members.client == null || members.client.Player == null)
                    continue;
                var member = members.client;
                if (member.Player.UID == user.Player.UID)
                    continue;
                if (member.Player.Map != leaderMap || member.Player.DynamicID != leaderDyn)
                    continue;
                if (IsJailMap(member.Player.Map) || !member.Player.Alive || member.Player.Level < MinTeamMemberLevel)
                    continue;
                if (Role.Core.GetDistance(leaderX, leaderY, member.Player.X, member.Player.Y) >= 18)
                    continue;
                member.Teleport((ushort)(x - rnd.Next(1, 6)), (ushort)(y - rnd.Next(1, 6)), mapId, dyn);
            }
        }

        public void JoinLayer(Client.GameClient user, ushort x, ushort y, byte layer, ServerSockets.Packet stream)
        {
            if (layer > MaxLayer || !CanEnterLayer(user, layer) || IsJailMap(user.Player.Map))
            {
                user.SendSysMesage("You haven't unlocked this floor of the Tower of Mystery yet.", MsgMessage.ChatMode.System);
                return;
            }
            uint leaderMap = user.Player.Map, leaderDyn = user.Player.DynamicID;
            ushort leaderX = user.Player.X, leaderY = user.Player.Y;
            user.Player.JoinTowerOfMysteryLayer = layer;
            user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");

            user.Player.TOM_StartChallenge = false;
            user.Player.TOM_FinishChallenge = false;
            switch (Math.Min(8, (int)layer))
            {
                case 0:
                    {
                        var DBmap = Database.Server.ServerMaps[4000];
                        uint Dyn = DBmap.GenerateDynamicID();
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, GreatPheasant(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, GreatPheasant(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 1:
                    {
                        var DBmap = Database.Server.ServerMaps[4000];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WingedLord(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WingedLord(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 2:
                    {
                        var DBmap = Database.Server.ServerMaps[4000];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WaterTerror(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WaterTerror(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 3:
                    {

                        var DBmap = Database.Server.ServerMaps[4003];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, RuthlessAsura(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, RuthlessAsura(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 4:
                    {
                        var DBmap = Database.Server.ServerMaps[4003];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, SoulStrangler(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, SoulStrangler(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 5:
                    {
                        var DBmap = Database.Server.ServerMaps[4003];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, DarkGlutton(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, DarkGlutton(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 6:
                    {
                        var DBmap = Database.Server.ServerMaps[4006];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, AlienDragon(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, AlienDragon(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 7:
                    {
                        var DBmap = Database.Server.ServerMaps[4006];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, ShadowSpider(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, ShadowSpider(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 8:
                    {
                        var DBmap = Database.Server.ServerMaps[4008];
                        uint Dyn = DBmap.GenerateDynamicID();
                        PullTeam(user, x, y, DBmap.ID, Dyn, leaderMap, leaderDyn, leaderX, leaderY);
                        user.Teleport(x, y, DBmap.ID, Dyn);
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, FlameGiant(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, FlameGiant(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
            }
        }
    }
}
