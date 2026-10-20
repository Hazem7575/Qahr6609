using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static class MsgTeam
    {
        public enum TeamTypes : uint
        {
            Create = 0,
            JoinRequest = 1,
            ExitTeam = 2,
            AcceptInvitation = 3,
            InviteRequest = 4,
            AcceptJoinRequest = 5,
            Dismiss = 6,
            Kick = 7,
            ForbidJoining = 8,
            UnforbidJoining = 9,
            LootMoneyOff = 10,
            LootMoneyOn = 11,
            LootItemsOff = 12,
            LootItemsOn = 13,
            RejectInvitation = 14,
            Send = 15
        }

        public static unsafe void GetTeamPacket(this ServerSockets.Packet stream, out TeamTypes Typ, out uint UID)
        {
            Typ = (TeamTypes)stream.ReadUInt32();
            UID = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet TeamCreate(this ServerSockets.Packet stream, TeamTypes Typ, uint UID)
        {
            stream.InitWriter();
            stream.Write((uint)Typ);
            stream.Write(UID);

            stream.Finalize(GamePackets.Team);
            return stream;
        }

        // Pending invitations / join requests: key = (kind, sender UID, receiver UID), value = time sent.
        // AcceptInvitation / AcceptJoinRequest are only honoured for a matching request younger than PendingTimeoutSec.
        // Generous enough for the client popup to sit on screen for a while before being answered.
        const int PendingTimeoutSec = 300;
        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> Pending
            = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

        static string PendingKey(TeamTypes kind, uint sender, uint receiver)
        {
            return ((uint)kind).ToString() + ":" + sender.ToString() + ":" + receiver.ToString();
        }
        internal static void AddPending(TeamTypes kind, uint sender, uint receiver)
        {
            if (Pending.Count > 10000)
            {
                foreach (var pair in Pending)
                {
                    if ((DateTime.Now - pair.Value).TotalSeconds > PendingTimeoutSec)
                    {
                        DateTime old;
                        Pending.TryRemove(pair.Key, out old);
                    }
                }
            }
            Pending[PendingKey(kind, sender, receiver)] = DateTime.Now;
        }
        static bool TakePending(TeamTypes kind, uint sender, uint receiver)
        {
            DateTime at;
            if (!Pending.TryRemove(PendingKey(kind, sender, receiver), out at))
                return false;
            return (DateTime.Now - at).TotalSeconds <= PendingTimeoutSec;
        }
        /// <summary>Team settings and kicks are reserved to the team leader.</summary>
        static bool IsLeader(Client.GameClient user)
        {
            return user.Team != null && user.Team.Leader != null && user.Team.TeamLider(user);
        }

        [PacketAttribute(GamePackets.Team)]
        private static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (!user.Player.OnMyOwnServer)
                return;
            TeamTypes Typ;
            uint UID;
            stream.GetTeamPacket(out Typ, out UID);


            if (user.Player.Map == 700)
            {

                return;
            }

            switch (Typ)
            {
                case TeamTypes.Create:
                    {
                        // A player already in a team cannot create another one.
                        if (user.Team != null && user.Team.IsTeamMember(user.Player.UID))
                            break;
                        user.Team = new Role.Instance.Team(user);
                        Typ = TeamTypes.Send;
                        user.Send(stream.TeamCreate(Typ, UID));
                        Typ = TeamTypes.Create;
                        user.Send(stream.TeamCreate(Typ, UID));

                        user.HeroRewards.AddGoal(203);

                        break;
                    }
                case TeamTypes.Dismiss:
                    {
                        if (user.Team != null)
                        {
                            if (user.InSkillTeamPk() || user.InTeamQualifier())
                            {
                                user.SendSysMesage("Sorry, invalid action during tournment.");
                                return;
                            }
                            user.Team.Remove(user, false);
                        }
                        else
                        {
                            user.SendSysMesage("Sorry, you are not in team!");
                          
                        }
                        break;
                    }
                case TeamTypes.Kick:
                    {
                        if (user.Team == null)
                        {
                            user.SendSysMesage("Sorry, you are not in team!");
                         
                            break;
                        }
                        // Only the leader kicks, never himself.
                        if (!IsLeader(user) || UID == user.Player.UID)
                            break;
                        Client.GameClient Kicker;
                        if (user.Team.TryGetMember(UID, out Kicker))
                        {
                            user.Team.Remove(Kicker, false);
                            if (Kicker.InSkillTeamPk())
                            {
                                Kicker.Teleport(334, 625, 1002);
                            }
                        }
                        break;
                    }
                case TeamTypes.ExitTeam:
                    {
                        if (user.InSkillTeamPk() || user.InTeamQualifier())
                        {
                            user.SendSysMesage("Sorry, invalid action during tournment.");
                            return;
                        }
                        if (user.Team != null)
                        {
                            user.Team.Remove(user, true);
                        }
                        break;
                    }
                case TeamTypes.InviteRequest:
                    {
                        if (user.InSkillTeamPk() || user.InTeamQualifier())
                        {
                            user.SendSysMesage("Sorry, invalid action during tournment.");
                            return;
                        }
                        if (user.Team != null)
                        {
                            if (user.Team.CkeckToAdd() && user.Team.TeamLider(user))
                            {
                                Role.IMapObj Invitee;
                                if (user.Player.View.TryGetValue(UID, out Invitee, Role.MapObjectType.Player))
                                {
                                    Role.Player Inv = Invitee as Role.Player;
                                    if (BlackList.Blocks(Inv.UID, user.Player.Name))
                                        break;//on the invitee's black list
                                    if (Inv.Owner.Team == null)
                                    {
                                        AddPending(TeamTypes.InviteRequest, user.Player.UID, Inv.UID);
                                        Inv.Send(stream.PopupInfoCreate(user.Player.UID, Inv.UID, user.Player.Level, user.Player.BattlePower));

                                        UID = user.Player.UID;
                                        Inv.Send(stream.TeamCreate(Typ, UID));
                                    }
                                    else
                                    {
                                        user.SendSysMesage(Inv.Name + " is already in a team.");
                                        
                                    }
                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.AcceptJoinRequest:
                    {
                        if (user.InSkillTeamPk() || user.InTeamQualifier())
                        {
                            user.SendSysMesage("Sorry, invalid action during tournment.");
                            return;
                        }
                        if (user.Team != null && user.Player.Alive)
                        {
                            if (user.Team.CkeckToAdd() && user.Team.TeamLider(user) && !user.Team.ForbidJoin)
                            {
                                Role.IMapObj obj;
                                if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                                {
                                    Client.GameClient NewTeammate = (obj as Role.Player).Owner;
                                    // The player must have asked to join this leader's team.
                                    if (!TakePending(TeamTypes.JoinRequest, NewTeammate.Player.UID, user.Player.UID))
                                    {
                                        user.SendSysMesage("The request has expired.");
                                        break;
                                    }
                                    if (NewTeammate.InSkillTeamPk() || NewTeammate.InTeamQualifier())
                                        break;
                                    if (NewTeammate.Team != null)
                                    {
                                        NewTeammate.SendSysMesage("You are already in a team.");
                                       
                                        return;
                                    }
                                    // Team.Add sets NewTeammate.Team itself, only once the member was really added.
                                    user.Team.Add(stream,NewTeammate);

                                }
                            }
                        }

                        break;
                    }
                case TeamTypes.AcceptInvitation:
                    {
                        if (user.InSkillTeamPk() || user.InTeamQualifier())
                        {
                            user.SendSysMesage("Sorry, invalid action during tournment.");
                            return;
                        }
                        if (user.Team == null && user.Player.Alive)
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Client.GameClient Leader = (obj as Role.Player).Owner;
                                if (Leader.Team != null)
                                {
                                    // Only an invitation this leader actually sent can be accepted.
                                    if (!TakePending(TeamTypes.InviteRequest, Leader.Player.UID, user.Player.UID))
                                    {
                                        user.SendSysMesage("The request has expired.");
                                        break;
                                    }
                                    if (!Leader.Team.TeamLider(Leader))
                                        break;
                                    if (!Leader.Team.CkeckToAdd() || Leader.Team.ForbidJoin)
                                        return;

                                    // Team.Add sets user.Team itself, only once the member was really added.
                                    Leader.Team.Add(stream, user);
                                }
                                else
                                {
                                    user.SendSysMesage(Leader.Player.Name + "'s doesn't have a team.");
                                   
                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.JoinRequest:
                    {
                        if (user.InSkillTeamPk() || user.InTeamQualifier())
                        {
                            user.SendSysMesage("Sorry, invalid action during tournment.");
                            return;
                        }
                        if (user.Team == null && user.Player.Alive)
                        {
                            Role.IMapObj obj;
                            if (user.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                Client.GameClient Leader = (obj as Role.Player).Owner;
                                if (Leader.Team != null)
                                {
                                    if (BlackList.Blocks(Leader.Player.UID, user.Player.Name))
                                        break;//on the leader's black list
                                    if (Leader.Team.TeamLider(Leader) && Leader.Team.CkeckToAdd())
                                    {
                                        AddPending(TeamTypes.JoinRequest, user.Player.UID, Leader.Player.UID);
                                        Leader.Send(stream.PopupInfoCreate(user.Player.UID, Leader.Player.UID, user.Player.Level, user.Player.BattlePower));

                                        UID = user.Player.UID;
                                        Leader.Send(stream.TeamCreate(Typ, UID));
                                    }
                                    else
                                    {
                                        user.SendSysMesage(Leader.Player.Name + "'s team is already full.");
                                  
                                    }
                                }
                                else
                                {
                                    user.SendSysMesage(Leader.Player.Name + "'s doesn't have a team.");
                                  
                                }
                            }
                        }
                        break;
                    }
                case TeamTypes.RejectInvitation:
                    {
                        // UID = the player who sent the invitation (or the join request) being turned down.
                        bool wasInvite = TakePending(TeamTypes.InviteRequest, UID, user.Player.UID);
                        bool wasJoin = !wasInvite && TakePending(TeamTypes.JoinRequest, UID, user.Player.UID);
                        if (!wasInvite && !wasJoin)
                            break;
                        Client.GameClient asker;
                        if (Database.Server.GamePoll.TryGetValue(UID, out asker) && asker.Player != null)
                        {
                            asker.Send(stream.TeamCreate(TeamTypes.RejectInvitation, user.Player.UID));
                            if (wasInvite)
                                asker.SendSysMesage(user.Player.Name + " rejected your team invitation.");
                            else
                                asker.SendSysMesage(user.Player.Name + " rejected your request to join the team.");
                        }
                        break;
                    }
                case TeamTypes.ForbidJoining:
                    {
                        if (IsLeader(user))
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.ForbidJoin = true;
                        }
                        break;
                    }
                case TeamTypes.UnforbidJoining:
                    {
                        if (IsLeader(user))
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.ForbidJoin = false;
                        }
                        break;
                    }
                case TeamTypes.LootMoneyOn:
                    {
                        if (IsLeader(user))
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupMoney = true;
                        }
                        break;
                    }
                case TeamTypes.LootMoneyOff:
                    {
                        if (IsLeader(user))
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupMoney = false;
                        }
                        break;
                    }
                case TeamTypes.LootItemsOn:
                    {
                        if (IsLeader(user))
                        {
                             user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupItems = true;
                        }
                        break;
                    }
                case TeamTypes.LootItemsOff:
                    {
                        if (IsLeader(user))
                        {
                            user.Team.SendTeam(stream.TeamCreate(Typ, UID), 0);
                            user.Team.PickupItems = false;
                        }
                        break;
                    }
            }
        }
    }

}
