using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Conquer6609.Game.MsgServer;

namespace Conquer6609.Role.Instance
{
    // Union / Kingdom management as in the official client (patch 6609):
    //  - membership is restored at login (BindPlayer) and saved with the character,
    //  - leadership transfer (abdicate), resigning an office,
    //  - official allowance (once a day, kingdom officials), Stipend Token (Emperor, once a week) and the stipend claim
    //    (free pack / 1350 War Exploits for 3 packs / 200 CPs for 5 packs, see client ini/Info.ini [OrderSalaryCost]),
    //  - Summon / Shift tokens (LeagueToken 102 / 204),
    //  - handing in carried Gold Bricks (MsgAction 436) at the own treasury.
    // Tunables live in shell.ini [Union].
    public partial class Union
    {
        #region settings
        static int Cfg(string key, int def)
        {
            return Conquer6609.Framework.Settings.Int("Union", key, def);
        }
        /// <summary>Gold Bricks a new union starts with (official: 500).</summary>
        public static uint FoundingGoldBricks { get { return (uint)Math.Max(0, Cfg("FoundingGoldBricks", 500)); } }
        /// <summary>CPs the Union Leader pays to rename the union (client ini/Info.ini [ChangeLeagueName] Cost=2700).</summary>
        public static uint RenameCost { get { return (uint)Math.Max(0, Cfg("RenameCost", 2700)); } }
        /// <summary>Stipend claim prices (client ini/Info.ini [OrderSalaryCost]).</summary>
        public static uint StipendExploitCost { get { return (uint)Math.Max(0, Cfg("StipendExploitCost", 1350)); } }
        public static uint StipendCpCost { get { return (uint)Math.Max(0, Cfg("StipendCpCost", 200)); } }
        /// <summary>Hours between two Stipend Tokens; a token can be claimed until the next one may be issued ("Once in a week").</summary>
        public static int StipendCooldownHours { get { return Math.Max(1, Cfg("StipendCooldownHours", 168)); } }
        public static int SummonCooldownMinutes { get { return Math.Max(0, Cfg("SummonCooldownMinutes", 30)); } }
        public static int SummonAnswerSeconds { get { return Math.Max(10, Cfg("SummonAnswerSeconds", 60)); } }
        /// <summary>Union Fund an allowance claim takes from the Treasury (0 = free). AllowanceFundCost applies to every office.</summary>
        public static uint AllowanceFundCost { get { return (uint)Math.Max(0, Cfg("AllowanceFundCost", 0)); } }
        #endregion

        public const uint SummonTokenID = 102, ShiftTokenID = 204, StipendTokenID = 205;

        /// <summary>Serial of the last Stipend Token (0 = none issued yet). Saved in Unions\UID.ini.</summary>
        public uint StipendSerial;
        /// <summary>When the last Stipend Token was issued (DateTime ticks, local time). Saved in Unions\UID.ini.</summary>
        public long StipendIssuedAt;
        /// <summary>Last Summon / Shift token issue time per token id (not saved: a cooldown only).</summary>
        readonly ConcurrentDictionary<uint, DateTime> TokenIssued = new ConcurrentDictionary<uint, DateTime>();

        public static uint DayStamp(DateTime when)
        {
            return (uint)(when.Year * 10000 + when.Month * 100 + when.Day);
        }

        #region members
        /// <summary>The union member with this UID (a member of one of the guilds or an individual member).</summary>
        public Member FindMember(uint uid)
        {
            Member member;
            if (Members.TryGetValue(uid, out member))
                return member;
            foreach (var guild in Guilds.Values)
            {
                Guild.Member gm;
                if (guild.Members.TryGetValue(uid, out gm) && gm.UnionMem != null)
                    return gm.UnionMem;
            }
            return null;
        }
        /// <summary>The guild of the union this UID belongs to, or null for an individual member.</summary>
        public Guild GuildOf(uint uid)
        {
            foreach (var guild in Guilds.Values)
                if (guild.Members.ContainsKey(uid))
                    return guild;
            return null;
        }
        /// <summary>All members of the union (guild members and individual members).</summary>
        public List<Member> AllMembers()
        {
            var list = new List<Member>(Members.Values);
            foreach (var guild in Guilds.Values)
                foreach (var gm in guild.Members.Values)
                    if (gm.UnionMem != null)
                        list.Add(gm.UnionMem);
            return list;
        }
        public IEnumerable<Client.GameClient> OnlineClients()
        {
            foreach (var client in Database.Server.GamePoll.Values)
                if (client.Player != null && client.Player.MyUnion == this && client.Player.UnionMemeber != null)
                    yield return client;
        }

        static bool NeedsBind(Client.GameClient client)
        {
            var p = client.Player;
            if (p.MyUnion == null || p.UnionMemeber == null)
                return true;
            return p.MyGuildMember != null && p.MyGuildMember.UnionMem != null && !object.ReferenceEquals(p.MyGuildMember.UnionMem, p.UnionMemeber);
        }

        /// <summary>Re-links a player whose union data is missing or stale (e.g. he just joined a guild of a union).</summary>
        public static void EnsureBound(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return;
            var p = client.Player;
            bool related = p.MyUnion != null || (p.MyGuild != null && p.MyGuild.UnionID != 0);
            if (related && NeedsBind(client))
                BindPlayer(client);
        }

        /// <summary>
        /// Links the player to his union: as a member of a guild of the union (Guild.Member.UnionMem) or as an
        /// individual member (Union.Members). Called at login and whenever a union packet finds the player unbound.
        /// </summary>
        public static bool BindPlayer(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return false;
            var p = client.Player;
            Union union = null;
            Member member = null;
            if (p.MyGuild != null && p.MyGuild.UnionID != 0)
            {
                var guildUnion = p.MyGuild.GetUnion;
                if (guildUnion != null && guildUnion.Guilds.ContainsKey(p.MyGuild.Info.GuildID))
                {
                    Guild.Member gm = p.MyGuildMember;
                    if (gm == null)
                        p.MyGuild.Members.TryGetValue(p.UID, out gm);
                    if (gm != null)
                    {
                        if (gm.UnionMem == null)
                            gm.UnionMem = Member.CreateMember(gm);
                        union = guildUnion;
                        member = gm.UnionMem;
                    }
                }
            }
            if (member == null)
            {
                foreach (var u in UnionPoll.Values)
                {
                    Member m;
                    if (u.Members.TryGetValue(p.UID, out m))
                    {
                        union = u;
                        member = m;
                        break;
                    }
                }
            }
            if (union == null || member == null)
            {
                p.MyUnion = null;
                p.UnionMemeber = null;
                return false;
            }
            member.UnionID = union.UID;
            member.ReceiveKick = 0;
            member.Owner = client;
            member.Name = p.Name;
            member.Level = p.Level;
            member.Class = p.Class;
            member.Mesh = p.Mesh;
            member.NobilityRank = p.NobilityRank;
            if (member.Rank == Member.MilitaryRanks.Emperor && union.EmperrorUID != p.UID)
            {
                // a stale Emperor rank (the throne moved on while he was away)
                member.Rank = Member.MilitaryRanks.Member;
                Member holder;
                if (union.UnionRanks.TryGetValue(Member.MilitaryRanks.Emperor, out holder) && holder.UID == p.UID)
                    union.UnionRanks.Remove(Member.MilitaryRanks.Emperor);
            }
            else if (union.EmperrorUID == p.UID && member.Rank != Member.MilitaryRanks.Emperor)
                union.AddEmperror(member);
            if (member.Rank == Member.MilitaryRanks.Emperor)
            {
                union.Emperor = p.Name;
                union.AddEmperror(member);
            }
            p.MyUnion = union;
            p.UnionMemeber = member;
            return true;
        }

        /// <summary>The saved union rank of a member, checked against the union (only the union's EmperrorUID is Emperor).</summary>
        public static Member.MilitaryRanks LoadedRank(Union union, uint uid, uint raw)
        {
            var rank = (Member.MilitaryRanks)raw;
            if (!Enum.IsDefined(typeof(Member.MilitaryRanks), rank))
                return Member.MilitaryRanks.Member;
            if (union.EmperrorUID == uid)
                return Member.MilitaryRanks.Emperor;
            if (rank == Member.MilitaryRanks.Emperor)
                return Member.MilitaryRanks.Member;
            return rank;
        }

        /// <summary>Sends the member's new rank (online) or saves it (offline).</summary>
        void Refresh(ServerSockets.Packet stream, Member member, bool resendView)
        {
            if (member.IsOnline && member.Owner.Player != null)
            {
                SendMyInfo(stream, member.Owner);
                if (resendView)
                    member.Owner.Player.View.ReSendView(stream);
            }
            else
                Database.ServerDatabase.LoginQueue.TryEnqueue(member);
        }

        /// <summary>Takes every office (core official, imperial guard, harem) away from the member.</summary>
        void RemoveOffices(Member member)
        {
            Member removed;
            ImperialGuards.TryRemove(member.UID, out removed);
            ImperialHarem.TryRemove(member.UID, out removed);
            foreach (var rank in UnionRanks.Keys.ToArray())
            {
                Member holder;
                if (rank != Member.MilitaryRanks.Emperor && UnionRanks.TryGetValue(rank, out holder) && holder.UID == member.UID)
                    UnionRanks.Remove(rank);
            }
        }

        public void SendUnionSysMessage(string text, MsgMessage.MsgColor color = MsgMessage.MsgColor.yellow)
        {
            foreach (var client in OnlineClients().ToArray())
                client.SendSysMesage(text, MsgMessage.ChatMode.System, color);
        }
        #endregion

        #region leadership
        /// <summary>The Union Leader / Emperor can hand over to the leader of a member guild or to a member without a guild.</summary>
        public bool CanLead(Member target)
        {
            var guild = GuildOf(target.UID);
            if (guild == null)
                return Members.ContainsKey(target.UID);
            Guild.Member gm;
            return guild.Members.TryGetValue(target.UID, out gm) && gm.Rank == Flags.GuildMemberRank.GuildLeader;
        }

        public void TransferLeader(ServerSockets.Packet stream, Member oldEmperor, Member target)
        {
            RemoveOffices(target);
            UnionRanks.Remove(Member.MilitaryRanks.Emperor);
            if (oldEmperor != null && oldEmperor.UID != target.UID)
            {
                oldEmperor.Rank = Member.MilitaryRanks.Member;
                Refresh(stream, oldEmperor, true);
            }
            target.Rank = Member.MilitaryRanks.Emperor;
            UnionRanks.Add(Member.MilitaryRanks.Emperor, target);
            Emperor = target.Name;
            EmperrorUID = target.UID;
            Refresh(stream, target, true);
            SendUnionSysMessage((oldEmperor != null ? oldEmperor.Name : "The leader") + " has abdicated the leadership of the "
                + (IsKingdom == 1 ? "Kingdom" : "Union") + " to " + target.Name + ".");
        }

        /// <summary>The member gives up his core office / imperial guard post.</summary>
        public bool Resign(Member member, bool guard)
        {
            if (guard)
            {
                if (!ImperialGuards.ContainsKey(member.UID) && member.Rank != Member.MilitaryRanks.Imprl_Guard)
                    return false;
            }
            else
            {
                Member holder;
                if (member.Rank < Member.MilitaryRanks.LeftPre || member.Rank > Member.MilitaryRanks.CO_Gen
                    || !UnionRanks.TryGetValue(member.Rank, out holder) || holder.UID != member.UID)
                    return false;
            }
            UpdateUnioOfficials(member, Member.MilitaryRanks.Member, true);
            return true;
        }

        /// <summary>Asks the Emperor of a new kingdom for the kingdom title (LeagueOpt 17 opens the title dialog).</summary>
        public void PromptKingdomTitle(ServerSockets.Packet stream)
        {
            if (IsKingdom == 0 || !(string.IsNullOrEmpty(Title) || Title == "None"))
                return;
            Client.GameClient emperor;
            if (Database.Server.GamePoll.TryGetValue(EmperrorUID, out emperor) && emperor.Player != null && emperor.Player.MyUnion == this)
                emperor.Send(stream.LeagueOptCreate(MsgLeagueOpt.ActionID.KingdomTitleGui, UID, 0, 0, ""));
        }
        #endregion

        #region allowance / stipend
        /// <summary>The allowance item of an office (client item descriptions: "(Privilege)...Allowance"), 0 = no allowance.</summary>
        public static uint AllowanceItemFor(Member.MilitaryRanks rank)
        {
            switch (rank)
            {
                case Member.MilitaryRanks.Emperor: return 3007259;// EmperorAllowance
                case Member.MilitaryRanks.Empress:
                case Member.MilitaryRanks.LeftPre:
                case Member.MilitaryRanks.RightPre:
                case Member.MilitaryRanks.LeftMar:
                case Member.MilitaryRanks.RightMar: return 3007260;// RoyalAllowance: Empress, Marshals and Premiers
                case Member.MilitaryRanks.SEC_Gen:
                case Member.MilitaryRanks.DEF_Gen:
                case Member.MilitaryRanks.PAC_Gen:
                case Member.MilitaryRanks.CO_Gen:
                case Member.MilitaryRanks.Conc: return 3007261;// NoblesAllowance: the four Generals and Concubines
                case Member.MilitaryRanks.Imprl_Guard: return 3007262;// GuardAllowance
                case Member.MilitaryRanks.SeniorConc: return 3007263;// SeniorConcubineAllowance
            }
            return 0;
        }

        public bool StipendActive
        {
            get
            {
                return StipendSerial != 0 && DateTime.Now < new DateTime(StipendIssuedAt).AddHours(StipendCooldownHours);
            }
        }
        /// <summary>Seconds until the Emperor may issue the next Stipend Token.</summary>
        public uint StipendCooldownLeft
        {
            get
            {
                if (StipendIssuedAt == 0)
                    return 0;
                double left = (new DateTime(StipendIssuedAt).AddHours(StipendCooldownHours) - DateTime.Now).TotalSeconds;
                return left <= 0 ? 0 : (uint)Math.Ceiling(left);
            }
        }
        public uint TokenCooldownLeft(uint token)
        {
            if (token == StipendTokenID)
                return StipendCooldownLeft;
            DateTime at;
            if (!TokenIssued.TryGetValue(token, out at))
                return 0;
            double left = (at.AddMinutes(SummonCooldownMinutes) - DateTime.Now).TotalSeconds;
            return left <= 0 ? 0 : (uint)Math.Ceiling(left);
        }

        /// <summary>
        /// LeagueInfo flag word: bit 0 = the official allowance can be claimed today, bit 1 = a stipend can be claimed
        /// (the client clears them itself after LeagueOpt 46 / 39).
        /// </summary>
        public uint InfoFlags(Member member)
        {
            uint flags = 0;
            if (member == null)
                return 0;
            if (IsKingdom == 1 && AllowanceItemFor(member.Rank) != 0 && member.AllowanceDay != DayStamp(DateTime.Now))
                flags |= 1;
            if (StipendActive && member.StipendClaimed != StipendSerial)
                flags |= 2;
            return flags;
        }

        public void SendInfo(ServerSockets.Packet stream, Client.GameClient user)
        {
            if (user == null || user.Player == null || user.Player.MyUnion != this)
                return;
            user.Send(stream.LeagueInfoCreate(Treasury, GoldBrick, InfoFlags(user.Player.UnionMemeber), Name, Emperor
                , Bulletin, Title, PlunderTarget, InvadingUnion));
        }

        /// <summary>Gives count items (bound) to the bag, or mails them when the bag is full. Returns 1 = bag, 2 = mailbox, 0 = failed.</summary>
        static uint Deliver(Client.GameClient client, ServerSockets.Packet stream, uint itemId, int count, string subject)
        {
            if (count <= 0 || !Database.Server.ItemsBase.ContainsKey(itemId))
                return 0;
            if (client.Inventory.HaveSpace((byte)count))
            {
                client.Inventory.Add(stream, itemId, (byte)count, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                return 1;
            }
            int sent = 0;
            string error = null;
            for (int i = 0; i < count; i++)
            {
                if (!Database.MailService.Send(client.Player.UID, "Kingdom", subject, "Your " + subject + ".", 0, 0, itemId, null, out error))
                    break;
                sent++;
            }
            if (sent == count)
                return 2;
            if (sent > 0)
            {
                MyConsole.WriteLine("[Union] " + client.Player.Name + ": only " + sent + "/" + count + " " + subject + " mailed (" + error + ")");
                return 2;
            }
            return 0;
        }

        /// <summary>LeagueOpt 46: a kingdom official claims today's allowance.</summary>
        public void ClaimAllowance(Client.GameClient client, ServerSockets.Packet stream)
        {
            var member = client.Player.UnionMemeber;
            if (member == null)
                return;
            uint item = AllowanceItemFor(member.Rank);
            if (IsKingdom != 1 || item == 0)
            {
                client.CreateBoxDialog("Only the Emperor, the Empress, the Core Officials, the Concubines and the Imperial Guards of a Kingdom can claim an allowance.");
                return;
            }
            uint today = DayStamp(DateTime.Now);
            if (member.AllowanceDay == today)
            {
                client.CreateBoxDialog("You`ve already claimed today`s allowance. Come back tomorrow.");
                return;
            }
            uint cost = AllowanceFundCost;
            if (cost > 0 && Treasury < cost)
            {
                client.CreateBoxDialog("The Treasury doesn`t have enough Union Fund to pay the allowance.");
                return;
            }
            uint where = Deliver(client, stream, item, 1, "Official Allowance");
            if (where == 0)
            {
                client.CreateBoxDialog("Your inventory and mailbox are full. Please make some room first.");
                return;
            }
            if (cost > 0)
                Treasury -= cost;
            member.AllowanceDay = today;
            member.RememberClaims();
            client.Send(stream.LeagueOptCreate(MsgLeagueOpt.ActionID.Allowance, UID, 0, where, ""));
            if (where == 2)
                client.SendSysMesage("Your inventory is full. The allowance was sent to your mailbox.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            SendInfo(stream, client);
        }

        /// <summary>LeagueOpt 39: claim the stipend of the current Stipend Token. option 1 = free pack, 2 = War Exploits for 3 packs, 3 = CPs for 5 packs.</summary>
        public void ClaimStipend(Client.GameClient client, ServerSockets.Packet stream, uint option)
        {
            var member = client.Player.UnionMemeber;
            if (member == null || option < 1 || option > 3)
                return;
            if (!StipendActive)
            {
                client.CreateBoxDialog("The Emperor hasn`t issued a Stipend Token yet.");
                return;
            }
            if (member.StipendClaimed == StipendSerial)
            {
                client.CreateBoxDialog("You`ve already claimed the stipend of this Stipend Token.");
                return;
            }
            int packs = option == 1 ? 1 : (option == 2 ? 3 : 5);
            if (option == 2 && client.Player.KingDomExploits < StipendExploitCost)
            {
                client.CreateBoxDialog("Not enough War Exploits to claim 3*Stipends.");
                return;
            }
            if (option == 3 && client.Player.ConquerPoints < StipendCpCost)
            {
                client.CreateBoxDialog("Not enough CPs to claim 5*Stipends.");
                return;
            }
            // (Rank)StipendPack: 3006994 = Common ... 3007007 = General in Chief
            uint rank = (uint)client.Player.ExploitsRank;
            if (rank > 13)
                rank = 13;
            uint pack = 3006994 + rank;
            uint where = Deliver(client, stream, pack, packs, "Stipend Pack");
            if (where == 0)
            {
                client.CreateBoxDialog("Your inventory and mailbox are full. Please make some room first.");
                return;
            }
            if (option == 2)
            {
                client.Player.KingDomExploits -= StipendExploitCost;
                if (client.Player.KingDomExploits < 200 && client.Player.ExploitsRank != 0)
                    client.Player.SetExploitsRank((Flags.ExploitsRank)0);// spending exploits may degrade the rank
            }
            else if (option == 3)
                client.Player.ConquerPoints -= StipendCpCost;
            member.StipendClaimed = StipendSerial;
            member.RememberClaims();
            client.Send(stream.LeagueOptCreate(MsgLeagueOpt.ActionID.Stipend, UID, 0, where, ""));
            SendInfo(stream, client);
        }

        /// <summary>LeagueOpt 38 with token 205: the Emperor issues a Stipend Token (kingdom only, once a week, no fund).</summary>
        public bool IssueStipend(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (IsKingdom != 1)
            {
                client.CreateBoxDialog("Only the Emperor of a Kingdom can issue a Stipend Token.");
                return false;
            }
            uint left = StipendCooldownLeft;
            if (left > 0)
            {
                client.CreateBoxDialog("The Stipend Token is cooling down. You can issue it again in " + (left / 3600) + " hour(s) " + (left % 3600 / 60) + " minute(s).");
                return false;
            }
            uint serial = (uint)(DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalSeconds;
            if (serial <= StipendSerial)
                serial = StipendSerial + 1;
            StipendSerial = serial;
            StipendIssuedAt = DateTime.Now.Ticks;
            foreach (var member in OnlineClients().ToArray())
            {
                member.SendSysMesage(client.Player.Name + " issued a Stipend Token! Open the Kingdom interface to claim your stipend.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                SendInfo(stream, member);
            }
            return true;
        }
        #endregion

        #region summon / shift tokens
        class PendingSummon
        {
            public uint Issuer;
            public uint Token;
            public DateTime Until;
        }
        // invited player UID -> the summon he may answer
        static readonly ConcurrentDictionary<uint, PendingSummon> Summons = new ConcurrentDictionary<uint, PendingSummon>();

        /// <summary>Maps the summon / shift token can not teleport to or from (events, jail, instances ...).</summary>
        public static bool IsSummonBlockedMap(Client.GameClient client)
        {
            uint map = client.Player.Map;
            return map == 1017 || map == 1081 || map == 2060 || map == 9972 || map == 1080 || map == 3820 || map == 3954 || map == 1806
                || map == 1505 || map == 1506 || map == 1507 || map == 1508 || map == 1509 || map == 1038 || map == 1768
                || map == 6000 || map == 6001 || map == 6002 || map == 6003 || map == 700
                || map == Game.MsgTournaments.MsgTeamDeathMatch.MapID || map == Game.MsgTournaments.MsgFootball.MapID
                || map == Game.MsgTournaments.MsgClassPKWar.MapID
                || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(map)
                || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(map)
                || client.Player.DynamicID != 0 || Program.BlockTeleportMap.Contains(map)
                || !client.Player.OnMyOwnServer;
        }

        static bool CanAnswerSummon(Client.GameClient client)
        {
            return client.Player.Level >= 120 && client.Player.Reborn >= 2;
        }

        /// <summary>LeagueToken (field1 = 0): issue a Summon (102: Emperor, Marshals / Union Leader) or Shift (204: Emperor, Imperial Guards) token.</summary>
        public void IssueSummon(Client.GameClient client, ServerSockets.Packet stream, uint token)
        {
            var me = client.Player.UnionMemeber;
            if (me == null)
                return;
            bool allowed;
            if (token == SummonTokenID)
                allowed = me.Rank == Member.MilitaryRanks.Emperor
                    || (IsKingdom == 1 && (me.Rank == Member.MilitaryRanks.LeftMar || me.Rank == Member.MilitaryRanks.RightMar));
            else
                allowed = me.Rank == Member.MilitaryRanks.Emperor && IsKingdom == 1;
            if (!allowed)
            {
                client.CreateBoxDialog(token == SummonTokenID ? "Only the Emperor, the Marshals or the Union Leader can issue a Summon Token."
                    : "Only the Emperor can issue a Shift Token.");
                return;
            }
            if (IsSummonBlockedMap(client))
            {
                client.CreateBoxDialog("You can`t issue this token here.");
                return;
            }
            uint left = TokenCooldownLeft(token);
            if (left > 0)
            {
                client.CreateBoxDialog("The token is cooling down. Try again in " + (left / 60 + 1) + " minute(s).");
                return;
            }
            TokenIssued[token] = DateTime.Now;
            int seconds = SummonAnswerSeconds;
            var until = DateTime.Now.AddSeconds(seconds);
            int invited = 0;
            foreach (var target in OnlineClients().ToArray())
            {
                if (target.Player.UID == client.Player.UID || !CanAnswerSummon(target))
                    continue;
                if (token == ShiftTokenID && !ImperialGuards.ContainsKey(target.Player.UID))
                    continue;
                Summons[target.Player.UID] = new PendingSummon() { Issuer = client.Player.UID, Token = token, Until = until };
                target.Send(stream.CreateLeagueToken(new MsgLeagueToken.LeagueToken()
                {
                    Type = 1,
                    dwpram2 = client.Player.UID,
                    dwpram3 = token,
                    Time = (uint)seconds,
                    text = client.Player.Name
                }));
                invited++;
            }
            client.SendSysMesage(invited == 0 ? "Nobody can answer the token right now." : invited + " hero(es) received your call.",
                MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
        }

        /// <summary>LeagueToken (field1 = 2): the invited player accepted; teleport him next to the issuer.</summary>
        public static void AnswerSummon(Client.GameClient client, uint issuer, uint token)
        {
            PendingSummon pending;
            if (!Summons.TryGetValue(client.Player.UID, out pending) || pending.Issuer != issuer || pending.Token != token)
                return;
            if (DateTime.Now > pending.Until)
            {
                Summons.TryRemove(client.Player.UID, out pending);
                client.SendSysMesage("The token has expired.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            Client.GameClient leader;
            if (!Database.Server.GamePoll.TryGetValue(issuer, out leader) || leader.Player == null || client.Player.MyUnion == null
                || leader.Player.MyUnion != client.Player.MyUnion)
            {
                Summons.TryRemove(client.Player.UID, out pending);
                return;
            }
            if (!client.Player.Alive || IsSummonBlockedMap(client) || IsSummonBlockedMap(leader) || !CanAnswerSummon(client))
            {
                client.SendSysMesage("You can`t go there with the teleport service of Summon Token.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            Summons.TryRemove(client.Player.UID, out pending);
            client.Teleport(leader.Player.X, leader.Player.Y, leader.Player.Map, leader.Player.DynamicID);
        }
        #endregion

        #region gold bricks
        static readonly int[] BrickFlags = { 161, 162, 163, 164, 165 };
        /// <summary>Gold Bricks one carried brick adds to the treasury, by status flag (Normal, Refined, Unique, Elite, Super).</summary>
        static uint BrickValue(int flag)
        {
            switch (flag)
            {
                case 161: return (uint)Math.Max(0, Cfg("GoldBrickNormal", 10));
                case 162: return (uint)Math.Max(0, Cfg("GoldBrickRefined", 30));
                case 163: return (uint)Math.Max(0, Cfg("GoldBrickUnique", 50));
                case 164: return (uint)Math.Max(0, Cfg("GoldBrickElite", 20));
                default: return (uint)Math.Max(0, Cfg("GoldBrickSuper", 100));
            }
        }

        /// <summary>The treasury dialog (MsgHandBrickInfo): kingdom = type 0, union = type 2, with today's plunder target.</summary>
        public void SendTreasury(ServerSockets.Packet stream, Client.GameClient client)
        {
            Union target = null;
            if (PlunderID != 0)
                UnionPoll.TryGetValue(PlunderID, out target);
            string kingdom = "", union = "";
            uint other = 0;
            if (target != null && target != this)
            {
                kingdom = string.IsNullOrEmpty(target.Title) || target.Title == "None" ? target.Name : target.Title;
                union = target.Name;
                other = target.GoldBrick;
            }
            client.Send(stream.HandBrickInfoCreate(IsKingdom == 1 ? MsgHandBrickInfo.BrickInfo.KingdomTreasury : MsgHandBrickInfo.BrickInfo.ShowSubmitDialog,
                GoldBrick, other, kingdom, union));
        }

        /// <summary>MsgAction 436: hand in the Gold Bricks the player carries (status flags 161-165) to his own treasury.</summary>
        public static void SubmitGoldBrick(Client.GameClient client, ServerSockets.Packet stream)
        {
            var p = client.Player;
            if (!p.InUnion)
            {
                client.SendSysMesage("You should join a Union before you can submit Gold Bricks.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            if (!p.OnMyOwnServer)
            {
                client.SendSysMesage("Bring the Gold Bricks back to your own server and hand them in there.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            var union = p.MyUnion;
            uint added = 0;
            int bricks = 0;
            foreach (int flag in BrickFlags)
            {
                var f = (MsgUpdate.Flags)flag;
                if (!p.ContainFlag(f))
                    continue;
                p.RemoveFlag(f);
                added += BrickValue(flag);
                bricks++;
            }
            if (bricks == 0)
            {
                client.SendSysMesage("You have no Gold Brick to submit.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                return;
            }
            union.GoldBrick = (uint)Math.Min(uint.MaxValue, (ulong)union.GoldBrick + added);
            client.SendSysMesage("You handed in " + bricks + " Gold Brick(s). " + added + " Gold Bricks were added to the treasury of " + union.Name + ".",
                MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            union.SendTreasury(stream, client);
        }
        #endregion
    }
}
