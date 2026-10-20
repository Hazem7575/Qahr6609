using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Security;

namespace Conquer6609.Game.MsgServer
{
    public unsafe  static class MsgGuildProces
    {
        public const int MaxBulletinLength = 255;

        /// <summary>Client supplied text: control characters (&lt; 0x20) are removed and the length is clamped.</summary>
        public static string SafeText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var sb = new StringBuilder(Math.Min(text.Length, maxLength));
            foreach (char c in text)
            {
                if (c < 0x20) continue;
                if (sb.Length >= maxLength) break;
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>A guild member may only discharge members of a lower rank: the leader anyone but himself,
        /// a deputy leader only ranks below deputy. The guild leader can never be discharged.</summary>
        public static bool CanDischarge(Role.Instance.Guild.Member actor, Role.Instance.Guild.Member target)
        {
            if (actor == null || target == null) return false;
            if (target.Rank == Role.Flags.GuildMemberRank.GuildLeader) return false;
            if (actor.UID == target.UID) return false;
            if (actor.Rank == Role.Flags.GuildMemberRank.GuildLeader) return true;
            if (actor.Rank == Role.Flags.GuildMemberRank.DeputyLeader)
                return (ushort)target.Rank < (ushort)Role.Flags.GuildMemberRank.DeputyLeader;
            return false;
        }

        public enum GuildAction : uint
        {
            JoinRequest = 1,
            AcceptRequest = 2,
            Quit = 3,
            InfoName = 6,
            Allied = 7,
            RemoveAlly = 8,
            Enemy = 9,
            RemoveEnemy = 10,
            SilverDonate = 11,
            Show = 12,
            Disband = 19,
            CpDonate = 20,
            RequestAllied = 23,
            Requirements = 24,
            Bulletin = 27,
            Promote = 28,
            ConfirmPromote = 29,
            Discharge = 30,
            Resign = 32,
            RequestPromote = 37,
            UpdatePromote = 38
        }

        public static void GuildRequest(this ServerSockets.Packet stream, out GuildAction requesttype, out uint UID, out int[] args, out string[] strlist)
        {
            requesttype = (GuildAction)stream.ReadInt32();
            UID = stream.ReadUInt32();
            args = new int[3];
            for (int i = 0; i < 3; i++)
            {
                args[i] = stream.ReadInt32();
            }
            strlist = stream.ReadStringList();
            stream.ReadBytes(3); //unknown
        }

        public static unsafe ServerSockets.Packet GuildRequestCreate(this ServerSockets.Packet stream, GuildAction requesttype, uint UID, int[] args, params string[] strlist)
        {
            stream.InitWriter();

            stream.Write((uint)requesttype);
            stream.Write(UID);
            stream.Write(args[0]);
            stream.Write(args[1]);
            stream.Write(args[2]);
            stream.Write(strlist);
            stream.ZeroFill(3);

            stream.Finalize(GamePackets.ProcesGuild);
            return stream;
        }

        [PacketAttribute(GamePackets.ProcesGuild)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
           
            if (!user.Player.OnMyOwnServer)
                return;
            GuildAction Action;
            uint UID;
            int[] args;
            string[] strlist;
            stream.GuildRequest(out Action, out UID, out args, out strlist);

            switch (Action)
            {
                case GuildAction.Resign:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        // The guild leader must transfer leadership instead of resigning.
                        if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            user.SendSysMesage("Transfer leadership first.");
                            break;
                        }

                        user.Player.MyGuild.Promote((uint)Role.Flags.GuildMemberRank.Member, user.Player, user.Player.Name, stream);
                        break;
                    }
                case GuildAction.InfoName:
                    {
                        if (user.OnInterServer)
                            break;
                        Role.Instance.Guild Guild;
                        if (Role.Instance.Guild.GuildPoll.TryGetValue(UID, out Guild))
                        {
                    
                            user.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.GuildName, Guild.Info.GuildID, true
                           , new string[1] { Guild.GuildName + " " + Guild.Info.LeaderName + " " + Guild.Info.Level + " " + Guild.Info.MembersCount });
                        }
                        break;
                    }
                case GuildAction.AcceptRequest:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Target = null;
                            Target = (obj as Role.Player).Owner;
                            if (Target == null) break;
                            if (Target.Player.MyGuild != null || Target.Player.MyGuildMember != null)
                                break;

                            if (Target.Player.TargetGuild == user.Player.UID)
                            {
                                // A join request can be accepted once.
                                Target.Player.TargetGuild = 0;
                      
                                user.Player.MyGuild.SendMessajGuild("" + user.Player.MyGuildMember.Rank.ToString() + " " + user.Player.Name + " of " + user.Player.MyGuild.GuildName + " agress " + Target.Player.Name+ " to join in.");
                                user.Player.MyGuild.AddPlayer(Target.Player,stream);
                            }
                            else
                            {
                                if (!user.Player.MyGuild.Recruit.Compare(Target.Player, Role.Instance.Guild.Recruitment.Mode.Requirements))
                                {
                                    Target.SendSysMesage("Sorry, Guild Recruitment wana block you join request");
                                  
                                    break;
                                }

                                Target.Send(stream.PopupInfoCreate(user.Player.UID, Target.Player.UID, user.Player.Level, user.Player.BattlePower));
                                Target.Send(stream.GuildRequestCreate(GuildAction.AcceptRequest, user.Player.UID, args, strlist));


                            }
                        }
                        break;
                    }
                case GuildAction.JoinRequest:
                    {
                        if (user.Player.MyGuild != null) break;
                        Role.IMapObj obj;
                        if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Client.GameClient Target = null;
                            Target = (obj as Role.Player).Owner;
                            if (Target == null) break;
                            if (Target.Player.MyGuild == null || Target.Player.MyGuildMember == null)
                                break;

                            if (!Target.Player.MyGuild.Recruit.Compare(user.Player, Role.Instance.Guild.Recruitment.Mode.Requirements))
                            {
                                user.SendSysMesage("Sorry, Guild Recruitment wana block you join request");
                            
                                break;
                            }
                            if (Target.Player.MyGuildMember.Accepter)
                            {
                                user.Player.TargetGuild = Target.Player.UID;
                               
                                Target.Send(stream.PopupInfoCreate(user.Player.UID, Target.Player.UID, user.Player.Level, user.Player.BattlePower));
                                Target.Send(stream.GuildRequestCreate(GuildAction.JoinRequest, user.Player.UID, args, strlist));
                            }
                        }
                        break;
                    }
                case GuildAction.SilverDonate:
                    {
                        // Donation amount: full value, 10000 .. MaxDonation, never more than the player owns.
                        long Amount;
                        if (!Guard.ValidAmount((ulong)UID, Guard.MaxDonation, out Amount))
                        {
                            Guard.Audit("guild-silver-donate", user.Player.UID, user.Player.Name, "rejected amount " + UID);
                            break;
                        }
                        if (Amount >= 10000)
                        {
                            if (user.InTrade || user.IsVendor || !user.Socket.Alive)
                                return;
                            if (user.Player.Money < Amount)
                                break;
                            if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                            {
                                if (user.Player.MyGuild.Info.SilverFund > long.MaxValue - Amount)
                                    break;
                                user.Player.Money -= Amount;
                                user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                                user.Player.MyGuildMember.MoneyDonate += Amount;
                                user.Player.MyGuild.Info.SilverFund += Amount;
                                user.Player.MyGuild.SendThat(user.Player);
                            }
                        }
                        break;
                    }
                case GuildAction.CpDonate:
                    {
                        // CP donation: full value, 1 .. MaxCpsAmount, never more than the player owns,
                        // and the 32-bit guild/member counters must not overflow.
                        long Amount;
                        if (!Guard.ValidAmount((ulong)UID, Guard.MaxCpsAmount, out Amount))
                        {
                            Guard.Audit("guild-cp-donate", user.Player.UID, user.Player.Name, "rejected amount " + UID);
                            break;
                        }
                        if (Amount >= 1)
                        {
                            if (user.InTrade || user.IsVendor || !user.Socket.Alive)
                                return;
                            if (user.Player.ConquerPoints < UID)
                                break;
                            if (user.Player.MyGuild != null && user.Player.MyGuildMember != null)
                            {
                                if ((ulong)user.Player.MyGuild.Info.ConquerPointFund + UID > uint.MaxValue
                                    || (ulong)user.Player.MyGuildMember.CpsDonate + UID > uint.MaxValue)
                                    break;
                                user.Player.ConquerPoints -= UID;

                                user.Player.MyGuildMember.CpsDonate += UID;
                                user.Player.MyGuild.Info.ConquerPointFund += UID;
                                user.Player.MyGuild.SendThat(user.Player);
                            }
                        }
                        break;
                    }
                case GuildAction.Show:
                    {
                        if (user.Player.MyGuild != null)
                        {
                            user.Player.MyGuild.SendThat(user.Player);
                        }
                        break;
                    }
                case GuildAction.Bulletin:
                    {
                        if (user.Player.MyGuild != null)
                        {
                            if (user.Player.Name != user.Player.MyGuild.Info.LeaderName)
                                break;
                            if (strlist != null && strlist.Length > 0 && strlist[0] != null)
                            {
                                string bulletin = SafeText(strlist[0], MaxBulletinLength);
                                if (Program.NameStrCheck(bulletin, false))
                                {
                                    user.Player.MyGuild.CreateBuletinTime();
                                    user.Player.MyGuild.Bulletin = bulletin;
                                    user.Player.MyGuild.SendThat(user.Player);
                                }
                                else
                                {
                                    user.SendSysMesage("Invalid Charasters in Bulletin.");
                                   
                                }
                            }
                        }
                        break;
                    }
                case GuildAction.Disband:
                    {
                        // Same rules as the Guild Director NPC (TwinCity GuildCreator option 2).
                        if (user.OnInterServer)
                            break;
                        if (user.Player.MyGuild == null || user.Player.MyGuildMember == null)
                        {
                            user.SendSysMesage("Sorry you not have guild.");
                            break;
                        }
                        if (user.Player.GuildRank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            user.SendSysMesage("Sorry you are not the Guild Leader.");
                            break;
                        }
                        if (user.Player.InUnion)
                        {
                            user.SendSysMesage("Please quit union first time.");
                            break;
                        }
                        user.Player.MyGuild.Dismis(user, stream);
                        break;
                    }
                case GuildAction.Quit:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            user.Player.MyGuild.Quit(user.Player.Name, false,stream);

                        }
                        break;
                    }
                case GuildAction.RequestPromote:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        SendPromote(stream, user, Action );
                        break;
                    }
                case GuildAction.Promote:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break; 
                        if (strlist != null && strlist.Length > 0 && strlist[0] != null)
                        {
                            user.Player.MyGuild.Promote(UID, user.Player, strlist[0],stream);
                        }
                        break;
                    }
                case GuildAction.RemoveAlly:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break; 
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            user.Player.MyGuild.RemoveAlly(strlist[0],stream);
                        }
                        break;
                    }
                case GuildAction.RequestAllied:
                    {
                         if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            string name = strlist[0];
                            if (name == user.Player.MyGuild.GuildName)
                                break;
                            if (!user.Player.MyGuild.IsEnemy(name))
                                user.Player.MyGuild.AddAlly(stream, name);
                            else
                            {
                                user.SendSysMesage("Soory, this guild is in ennemy`s list");

                            }
                        }
                       
                        break;
                    }
                case GuildAction.Allied:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break; 
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            string name = strlist[0];
                            if (name == user.Player.MyGuild.GuildName)
                                break;
                            if (!user.Player.MyGuild.IsEnemy(name))
                            {
                                var leader = Role.Instance.Guild.GetLeaderGuild(name);
                                if (leader != null && leader.IsOnline)
                                {
                                    Client.GameClient LeaderClient;
                                    if (Database.Server.GamePoll.TryGetValue(leader.UID, out LeaderClient))
                                    {
                                        LeaderClient.Send(stream.GuildRequestCreate(GuildAction.RequestAllied, 0, new int[3], user.Player.MyGuild.GuildName));
                                        user.Player.MyGuild.AddAlly(stream, name);
                                    }
                                }
                            }
                            else
                            {
                                user.SendSysMesage("Soory, this guild is in ennemy`s list");

                            }
                        }
                        break;
                    }
                case GuildAction.Enemy:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            string name = strlist[0];
                            if (name == user.Player.MyGuild.GuildName)
                                break;
                            if (user.Player.MyGuild.AllowAddAlly(name))
                            {
                                user.Player.MyGuild.AddEnemy(stream, name);
                                user.Player.MyGuild.SendMessajGuild("Guild Leader " + user.Player.Name + " has added Guild " + name + " to the enemy list!");
                                
                            }
                            else
                            {
                                user.SendSysMesage("Soory, this guild is in Ally`s list");
                              
                            }
                        }
                        break;
                    }
                case GuildAction.RemoveEnemy:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break; 
                        if (strlist.Length > 0 && strlist[0] != null)
                        {
                            user.Player.MyGuild.RemoveEnemy(strlist[0],stream);
                        }
                        break;
                    }
                case GuildAction.Discharge:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (strlist == null || strlist.Length == 0 || strlist[0] == null) break;
                        // Only the leader (any lower rank) or a deputy leader (ranks below deputy) may discharge.
                        var dischargeTarget = user.Player.MyGuild.GetMember(strlist[0]);
                        if (!CanDischarge(user.Player.MyGuildMember, dischargeTarget)) break;

                        user.Player.MyGuild.Discharge(strlist[0], stream);
                        break;
                    }
                case GuildAction.Requirements:
                    {
                        if (user.Player.MyGuild == null) break;
                        if (user.Player.MyGuildMember == null) break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;

                        user.Player.MyGuild.Recruit.Level = (byte)args[0];
                        user.Player.MyGuild.Recruit.Reborn = (byte)args[1];
                        user.Player.MyGuild.Recruit.SetFlag((int)args[2], Role.Instance.Guild.Recruitment.Mode.Requirements);
                        break;
                    }
                case GuildAction.UpdatePromote:
                    {
                        if (user.Player.MyGuild == null)
                            break;
                        if (user.Player.MyGuildMember == null)
                            break;

                        if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.DeputyLeader
                        || mem.Rank == Role.Flags.GuildMemberRank.Aide || mem.Rank == Role.Flags.GuildMemberRank.Steward || mem.Rank == Role.Flags.GuildMemberRank.Follower).ToArray();

                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));
                        }
                        else if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.LeaderSpouse)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.DeputyLeader
                       || mem.Rank == Role.Flags.GuildMemberRank.Steward || mem.Rank == Role.Flags.GuildMemberRank.Follower).ToArray();
                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));

                        }
                        else if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.Manager)
                        {
                            Role.Instance.Guild.Member[] Members = user.Player.MyGuild.Members.Values.Where((mem) => mem.Rank == Role.Flags.GuildMemberRank.Aide).ToArray();
                            user.Send(stream.GuildRankListCreate(MsgGuildMembers.Action.ListRanks, user.Player.MyGuild, Members));
                        }
                        break;
                    }

            }
        }
        private static string CreatePromotionString(StringBuilder builder, Role.Flags.GuildMemberRank rank, int occupants,
           int maxOccupants, int extraBattlePower, int conquerPoints)
        {
            builder.Remove(0, builder.Length);
            builder.Append((int)rank);
            builder.Append(" ");
            builder.Append(occupants);
            builder.Append(" ");
            builder.Append(maxOccupants);
            builder.Append(" ");
            builder.Append(extraBattlePower);
            builder.Append(" ");
            builder.Append(conquerPoints);
            builder.Append(" ");
            return builder.ToString();
        }

        public static unsafe void SendPromote(ServerSockets.Packet stream, Client.GameClient client, GuildAction typ)
        {
            if (client.Player.MyGuild == null) return;
            if (client.Player.MyGuildMember == null) return;
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            #region Guild Leader
            if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
            {
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.GuildLeader, 1, 1, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.GuildLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Aide, (int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Aide], 6, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Aide), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.DeputyLeader, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.DeputyLeader], 8, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.DeputyLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Steward, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Steward], 3, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Steward), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Follower, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Follower], 10, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Follower), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Member, (int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Member], (int)300, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Member), 0));
            }
            #endregion
            #region Leader's Spouse
            if (client.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.LeaderSpouse)
            {
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.DeputyLeader, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.DeputyLeader], 4, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.DeputyLeader), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Steward, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Steward], 3, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Steward), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Follower, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Follower], 10, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Follower), 0));
                list.Add(CreatePromotionString(builder, Role.Flags.GuildMemberRank.Member, (int)(int)client.Player.MyGuild.RanksCounts[(ushort)Role.Flags.GuildMemberRank.Member], (int)300, (int)client.Player.MyGuild.ShareMemberPotency(Role.Flags.GuildMemberRank.Member), 0));
            }
            #endregion
            int extraLength = 0;
            foreach (var str in list) extraLength += str.Length + 1;

            client.Send(stream.GuildRequestCreate(typ, 0, new int[3], list.ToArray()));
        }
    }
}
