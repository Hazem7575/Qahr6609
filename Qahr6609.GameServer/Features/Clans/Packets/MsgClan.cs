using Conquer6609.Game.Conquer6609_Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {


        public static unsafe ServerSockets.Packet ClanMembersCreate(this ServerSockets.Packet stream, Role.Instance.Clan.Member[] Elements)
        {
            stream.InitWriter();

            stream.Write((uint)MsgClan.Info.Members);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Write((uint)Elements.Length);
         
            foreach (var member in Elements)
            {
                stream.Write(member.Name, 16);
                stream.Write((uint)member.Level);
                stream.Write((ushort)member.Rank);
                stream.Write((ushort)(member.Online ? 1 : 0));
                stream.Write((uint)member.Class);
                stream.Write((uint)1);//unknow
                stream.Write(member.Donation);
            }
            stream.Finalize(GamePackets.Clan);
            return stream;
        }

        public static unsafe ServerSockets.Packet ClanBulletinCreate(this ServerSockets.Packet stream, Client.GameClient client, Role.Instance.Clan clan)
        {
            stream.InitWriter();

            stream.Write((uint)MsgClan.Info.SetAnnouncement);
            stream.Write(client.Player.ClanUID);
            stream.Write((uint)0);
            stream.Write(clan.ClanBuletin);

            stream.Finalize(GamePackets.Clan);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanRelation(this ServerSockets.Packet stream, List<Role.Instance.Clan> clans, MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);//4 ~ 5
            stream.ZeroFill(8);
            stream.Write((uint)clans.Count);
            foreach (var clan in clans)
            {
                stream.Write(clan.ID);
                stream.Write(clan.Name, 36);
                stream.Write(clan.LeaderName, 16);
            }
            stream.Finalize(GamePackets.Clan);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanRelationCreate(this ServerSockets.Packet stream, uint ClanID, string ClanName, string ClanLeader, MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);//4 ~ 8
            stream.Write(ClanID);//8 ~ 12
            stream.Write((uint)0);//12 ~ 16
            stream.Write(ClanName, ClanLeader);
            stream.Finalize(GamePackets.Clan);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanAppendSingleClientCreate(this ServerSockets.Packet stream, uint ClanID, string Name, MsgClan.Info aTyp)
        {
            stream.InitWriter();
            stream.Write((uint)aTyp);
            stream.Write(ClanID);
            stream.Write((uint)0);
            stream.Write(Name);
            stream.Finalize(GamePackets.Clan);
            return stream;
        }
        public static unsafe ServerSockets.Packet ClanCreate(this ServerSockets.Packet stream, Client.GameClient client, Role.Instance.Clan clan)
        {
            stream.InitWriter();
            string info = client.Player.MyClan.ID
   + " " + client.Player.MyClan.Members.Count + " 0 "
   + client.Player.MyClan.Donation + " "
   + client.Player.MyClan.Level + " "
   + (byte)client.Player.MyClan.Members[client.Player.UID].Rank + " 0 " + client.Player.MyClan.BP + " 0 0 0 "
   + client.Player.MyClan.Members[client.Player.UID].Donation;


            string ClanWarInfo = "0 0 0 0 0 0 0";

            string CurrentMap = "";
            string domination_map = "";
            var war = Game.MsgTournaments.MsgSchedules.ClanWar.GetWinnerWar(clan.ID);
            if (war != null && war.Proces == MsgTournaments.ProcesType.Dead)
            {
                CurrentMap = war.BestWinner.Name;  //war.Type.ToString() + "CityMap";
                domination_map = war.Type.ToString() + "CityMap";
            }
            stream.Write((uint)MsgClan.Info.Info);
            stream.Write(client.Player.ClanUID);
            stream.Write((uint)0);
            stream.Write(info, clan.Name, clan.LeaderName, ClanWarInfo, CurrentMap, domination_map);

            stream.Finalize(GamePackets.Clan);
            return stream;
        }

        public static unsafe void GetClan(this ServerSockets.Packet stream, out MsgClan.Info Mode, out uint UID, out uint dwparam, out string[] list)
        {
            Mode = (MsgClan.Info)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            dwparam = stream.ReadUInt32();

            list = stream.ReadStringList();
        }
        public static unsafe ServerSockets.Packet ClanCallBackCreate(this ServerSockets.Packet stream, MsgClan.Info Mode, uint UID, uint dwparam, string[] list)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(UID);
            stream.Write(dwparam);

            if (list != null)
            {
                stream.Write(list);
            }

            stream.Finalize(GamePackets.Clan);
            return stream;
        }
    }
    public unsafe struct MsgClan
    {
        public enum Info : uint
        {
            Info = 1,
            Members = 4,
            Recruit = 9,
            AcceptRecruit = 10,
            Join = 11,
            AcceptJoinRequest = 12,
            AddEnemy = 14,
            DeleteEnemy = 15,
            AddAlly = 19,
            RequestAlly = 17,
            AcceptAlly = 18,
            DeleteAlly = 20,
            TransferLeader = 21,
            KickMember = 22,
            Quit = 23,
            Announce = 24,
            SetAnnouncement = 25,
            Dedicate = 26,
            MyClan = 29,
            LoginAlly = 16,
            LoginEnemy = 13
        }

        // Pending clan invitations (Recruit) and join requests (Join): key = (kind, sender UID, receiver UID).
        // AcceptRecruit / AcceptJoinRequest are only honoured for a matching request younger than PendingTimeoutSec.
        // Generous enough for the client popup to sit on screen for a while before being answered.
        const int PendingTimeoutSec = 300;
        // Clan bulletin: characters kept from the client text, and a byte cap for the length-prefixed packet string.
        const int MaxClanBulletinLength = 128;
        const int MaxClanBulletinBytes = 250;
        static readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> Pending
            = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();

        static string PendingKey(Info kind, uint sender, uint receiver)
        {
            return ((uint)kind).ToString() + ":" + sender.ToString() + ":" + receiver.ToString();
        }
        static void AddPending(Info kind, uint sender, uint receiver)
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
        static bool TakePending(Info kind, uint sender, uint receiver)
        {
            DateTime at;
            if (!Pending.TryRemove(PendingKey(kind, sender, receiver), out at))
                return false;
            return (DateTime.Now - at).TotalSeconds <= PendingTimeoutSec;
        }

        [PacketAttribute(GamePackets.Clan)]
        private static unsafe void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.OnMyOwnServer)
                return;
            MsgClan.Info Mode;
            uint UID;
            uint dwparam;
            string[] list;

            stream.GetClan(out Mode, out UID, out dwparam, out list);
            if (list == null)
                list = new string[0];

            switch (Mode)
            {
                case Info.AcceptAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (UID == 1)
                        {
                            if (client.Player.MyClan.Ally.Count >= 5)
                            {
                                client.SendSysMesage("The amount of Allies have exceeded the maximum amount.");
                                break;
                            }
                            Role.Instance.Clan clan;
                            uint requestAlly = client.Player.MyClan.RequestAlly;
                            // An alliance request can be accepted once.
                            client.Player.MyClan.RequestAlly = 0;
                            if (requestAlly != 0 && requestAlly != client.Player.MyClan.ID
                                && Role.Instance.Clan.Clans.TryGetValue(requestAlly, out clan))
                            {
                                if (client.Player.MyClan.Ally.ContainsKey(clan.ID))
                                {
                                    client.SendSysMesage("This clan already is it on your Ally list .");
                                    break;
                                }
                                if (client.Player.MyClan.Enemy.ContainsKey(clan.ID))
                                {
                                    client.SendSysMesage("This clan already is on your Enemy list.");
                                    break;
                                }

                                if (client.Player.MyClan.Ally.TryAdd(clan.ID, clan))
                                {
                                    if (clan.Ally.TryAdd(client.Player.MyClan.ID, client.Player.MyClan))
                                    {
                                        client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.AddAlly));
                                        clan.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.AddAlly));
                                    }
                                }
                            }
                        }
                        else
                            client.Player.MyClan.RequestAlly = 0;
                        break;
                    }
                case Info.RequestAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (client.Player.MyClan.Ally.Count >= 5)
                        {
                            client.SendSysMesage("The amount of Allys have exceeded the maximum amount.");
                            break;
                        }

                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            Role.Player player = obj as Role.Player;
                            if (player.MyClan == null)
                                break;
                            if (player.MyClanMember == null) break;
                            if (player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                            if (client.Player.MyClan.Ally.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is it on your Ally list .");
                                break;
                            }
                            if (client.Player.MyClan.Enemy.ContainsKey(player.MyClan.ID))
                            {
                                client.SendSysMesage("This clan already is on your Enemy list.");
                                break;
                            }

                            player.MyClan.RequestAlly = client.Player.MyClan.ID;


                            player.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.MyClan.LeaderName, Info.RequestAlly));

                        }
                        break;
                    }
                case Info.DeleteAlly:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (client.Player.MyClan.IsAlly(Name))
                            {
                                Role.Instance.Clan clan;
                                if (client.Player.MyClan.Ally.TryRemove(client.Player.MyClan.GetClanAlly(Name), out clan))
                                {
                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, "", Info.DeleteAlly));

                                    Role.Instance.Clan ally_clan;
                                    if (clan.Ally.TryRemove(client.Player.MyClan.ID, out ally_clan))
                                    {
                                        clan.Send(stream.ClanRelationCreate(client.Player.MyClan.ID, client.Player.MyClan.Name, "", Info.DeleteAlly));
                                    }
                                }
                            }
                            else
                            {
                                client.SendSysMesage("This clan is not on your allies list .");
                            }
                        }
                        break;
                    }
                case Info.DeleteEnemy:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (client.Player.MyClan.IsEnemy(Name))
                            {
                                Role.Instance.Clan clan;
                                if (client.Player.MyClan.Enemy.TryRemove(client.Player.MyClan.GetClanEnemy(Name), out clan))
                                {
                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.DeleteEnemy));
                                }
                            }
                            else
                            {
                                client.SendSysMesage("This clan is not on your enemy list.");
                            }
                        }
                        break;
                    }
                case Info.AddEnemy:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (Name == client.Player.MyClan.Name)
                            {
                                client.SendSysMesage("You can't use your own clan name at enemy list.");
                                break;
                            }
                            if (client.Player.MyClan.Enemy.Count >= 5)
                            {
                                client.SendSysMesage("You can't have more than 5 Enemy's .");
                                break;
                            }
                            if (client.Player.MyClan.IsAlly(Name))
                            {
                                client.SendSysMesage("This clan belongs already to Ally's you can't add them to Enemys.");
                                break;
                            }
                            if (client.Player.MyClan.IsEnemy(Name))
                            {
                                client.SendSysMesage("This clan is already on your Enemy list.");
                                break;
                            }
                            Role.Instance.Clan clan;
                            if (client.Player.MyClan.TryGetClan(Name, out clan))
                            {
                                if (client.Player.MyClan.Enemy.TryAdd(clan.ID, clan))
                                {

                                    client.Player.MyClan.Send(stream.ClanRelationCreate(clan.ID, clan.Name, clan.LeaderName, Info.AddEnemy));
                                }
                            }
                        }
                        break;
                    }
                case Info.KickMember:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (Name == client.Player.Name)
                                break;
                            client.Player.MyClan.RemoveMember(Name, stream);
                            goto case Info.Members;
                        }
                        break;
                    }
                case Info.AcceptRecruit:
                    {
                        if (list.Length > 0)
                        {
                            if (client.Player.MyClan != null) break;
                            if (client.Player.MyClanMember != null) break;

                            Role.IMapObj obj;
                            if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                if (obj == null) break;
                                Role.Player Leader = obj as Role.Player;
                                if (Leader == null) break;
                                if (Leader.MyClan == null) break;
                                if (Leader.MyClanMember == null) break;
                                if (Leader.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                                // Only an invitation this clan leader actually sent can be accepted.
                                if (!TakePending(Info.Recruit, Leader.UID, client.Player.UID))
                                {
                                    client.SendSysMesage("The request has expired.");
                                    break;
                                }
                                if (Leader.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(Leader.MyClan.Level))
                                {
                                    Leader.Owner.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(Leader.MyClan.Level) + ".");
                                    break;
                                }

                                Leader.MyClan.AddMember(client, Role.Instance.Clan.Ranks.Member, stream);
                            }
                        }
                        break;
                    }
                case Info.Recruit:
                    {
                        if (client.Player.MyClan == null) break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (client.Player.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                        {
                            client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                            break;
                        }
                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            if (obj == null) break;
                            Role.Player recuiter = obj as Role.Player;
                            if (recuiter == null) break;
                            if (recuiter.MyClan != null) break;
                            if (recuiter.MyClanMember != null) break;

                            AddPending(Info.Recruit, client.Player.UID, recuiter.UID);
                            recuiter.Owner.Send(stream.ClanRelationCreate(client.Player.UID, client.Player.MyClan.Name, client.Player.Name, Info.Recruit));
                        }
                        break;
                    }
                case Info.AcceptJoinRequest:
                    {
                        if (list.Length > 0)
                        {
                            if (client.Player.MyClan == null) break;
                            if (client.Player.MyClanMember == null) break;
                            if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                            if (client.Player.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level))
                            {
                                client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
                                break;
                            }

                            Role.IMapObj obj;
                            if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                            {
                                if (obj == null) break;
                                Role.Player member = obj as Role.Player;
                                if (member == null) break;
                                if (member.MyClan != null) break;
                                if (member.MyClanMember != null) break;
                                // The player must have asked to join this clan leader.
                                if (!TakePending(Info.Join, member.UID, client.Player.UID))
                                {
                                    client.SendSysMesage("The request has expired.");
                                    break;
                                }

                                client.Player.MyClan.AddMember(member.Owner, Role.Instance.Clan.Ranks.Member, stream);

                            }
                        }
                        else
                        {

                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        }
                        break;
                    }
                case Info.Join:
                    {
                        if (client.Player.MyClan != null) break;
                        if (client.Player.MyClanMember != null) break;
                        Role.IMapObj obj;
                        if (client.Player.View.TryGetValue(UID, out obj, Role.MapObjectType.Player))
                        {
                            if (obj == null) break;
                            Role.Player Leader = obj as Role.Player;
                            if (Leader == null) break;
                            if (Leader.MyClan == null) break;
                            if (Leader.MyClanMember == null) break;
                            if (Leader.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                            AddPending(Info.Join, client.Player.UID, Leader.UID);
                            Leader.Owner.Send(stream.ClanAppendSingleClientCreate(client.Player.UID, client.Player.Name, Info.Join));
                        }

                        break;
                    }
                case Info.TransferLeader:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;

                        if (list.Length > 0)
                        {
                            string Name = list[0];
                            if (Name == client.Player.Name)
                                break;
                            Role.Instance.Clan.Member member;
                            if (client.Player.MyClan.TryGetMember(Name, out member))
                            {
                                Client.GameClient pClient;
                                if (Database.Server.GamePoll.TryGetValue(member.UID, out pClient))
                                {
                                    if (pClient.Player.MyClan == null) break;
                                    if (pClient.Player.MyClanMember == null) break;
                                    if (pClient.Player.ClanUID != client.Player.ClanUID) break;
                                    pClient.Player.MyClan.LeaderName = pClient.Player.Name;
                                    pClient.Player.ClanRank = (ushort)Role.Instance.Clan.Ranks.Leader;
                                    pClient.Player.MyClanMember.Rank = Role.Instance.Clan.Ranks.Leader;
                                    pClient.Player.MyClan.SendThat(stream, pClient);

                                    client.Player.MyClanMember.Rank = Role.Instance.Clan.Ranks.Member;
                                    client.Player.ClanRank = (ushort)Role.Instance.Clan.Ranks.Member;
                                    client.Player.MyClan.SendThat(stream, client);

                                    client.Player.View.ReSendView(stream);
                                    pClient.Player.View.ReSendView(stream);
                                }
                            }
                        }
                        break;
                    }
                case Info.Quit:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        // The leader cannot walk out of a clan that still has members; transfer leadership first.
                        if (client.Player.MyClanMember.Rank == Role.Instance.Clan.Ranks.Leader && client.Player.MyClan.Members.Count > 1)
                        {
                            client.SendSysMesage("Transfer leadership first.");
                            break;
                        }
                        client.Player.MyClan.RemoveMember(client);

                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        break;
                    }
                case Info.Dedicate:
                    {

                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.InTrade || client.IsVendor || !client.Socket.Alive)
                            return;
                        // Donation: full value, positive, within the cap, owned by the player and
                        // without overflowing the 32-bit clan/member donation counters.
                        long Amount;
                        if (!Conquer6609.Security.Guard.ValidAmount((ulong)UID, Conquer6609.Security.Guard.MaxDonation, out Amount))
                        {
                            Conquer6609.Security.Guard.Audit("clan-donate", client.Player.UID, client.Player.Name, "rejected amount " + UID);
                            break;
                        }
                        if ((ulong)client.Player.MyClan.Donation + UID > uint.MaxValue
                            || (ulong)client.Player.MyClanMember.Donation + UID > uint.MaxValue)
                            break;
                        if (client.Player.Money >= Amount)
                        {
                            client.Player.Money -= Amount;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.MyClan.Donation += (uint)UID;
                            client.Player.MyClanMember.Donation += (uint)UID;

                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));

                            client.Player.MyClan.SendThat(stream, client);
                        }
                        break;
                    }
                case Info.SetAnnouncement:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        if (client.Player.MyClanMember == null) break;
                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader) break;
                        if (list.Length > 0)
                        {
                            string Name = MsgGuildProces.SafeText(list[0], MaxClanBulletinLength);

                            // Free text (spaces allowed); NameStrCheck(.., false) still rejects the '/' save delimiter.
                            if (Program.NameStrCheck(Name, false) && global::Conquer6609.AnsiText.Value.GetByteCount(Name) <= MaxClanBulletinBytes)
                            {
                                client.Player.MyClan.ClanBuletin = Name;
                                client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                            }
                            else
                            {
                                client.SendSysMesage("Your buletin is too long or contains invalid characters (/ \\ ' { } [ ]).");
                            }
                        }
                        break;
                    }
                case Info.Members:
                    {
                        if (client.Player.MyClan == null)
                            break;
                        Role.Instance.Clan.Member[] members = client.Player.MyClan.Members.Values.ToArray();
                        Array.Sort(members, (c1, c2) =>
                        {
                            int val = c2.Online.CompareTo(c1.Online);
                            if (c1.Online == c2.Online)
                            {
                                val = c1.Rank.CompareTo(c2.Rank);
                                if (c1.Rank == c2.Rank)
                                    val = c1.Level.CompareTo(c2.Level);
                            }
                            return val;
                        });
                        client.Send(stream.ClanMembersCreate(members));
                        break;
                    }
                case Info.Announce:
                    {
                        client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        break;
                    }
                case Info.MyClan:
                    {
                        if (client.Player.MyClan != null)
                        {
                            try
                            {
                                client.Send(stream.ClanCreate(client, client.Player.MyClan));
                                string ExtraInfo = "0 0 0 0 0 0 0";
                                var war = Game.MsgTournaments.MsgSchedules.ClanWar.GetWinnerWar(client.Player.ClanUID);
                                if (war != null && war.Proces == MsgTournaments.ProcesType.Dead)
                                {
                                    ExtraInfo = "1 " + war.Winner.OccupationDays + " "
                               + war.Winner.Reward + " " + war.Winner.NextReward + " 0 0 0";
                                }
                                client.Send(stream.ClanAppendSingleClientCreate(client.Player.MyClan.ID, ExtraInfo/*for clan war*/, Info.MyClan));
                                client.Player.MyClan.SendBuletin(stream, client);
                            }
                            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            client.Send(stream.ClanCallBackCreate(Mode, UID, dwparam, list));
                        }
                        break;
                    }
            }
        }

    }
}
