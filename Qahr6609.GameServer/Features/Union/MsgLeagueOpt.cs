using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgServer
{
  public static  class MsgLeagueOpt
    {
      public enum ActionID : byte
      {
          Info = 0,
          SynList = 1,
          Members = 2,
          ViewOthers = 3,
          List = 4,
          MyUnion = 5,
          NewUnionName = 6,
          Create = 7,
          Pledge = 8,
          Quit = 9,
          ExpelMember = 10,//
          GuildPledge = 11,
          GuildQuit = 12,
          ExpelGuild = 13,
          TransferLeader = 14,
          KingdomRank = 15,
          UnionRank = 16,
          KingdomTitleGui = 17,//
          NewUnionTitle = 18,
          RecruitDeclaration = 20,
          NewNameDone = 22,
          ImperialGuards = 23,
          AppointImperialGuard = 24,//
          RemovePalaceGuard = 25,//
          ReSignPalaceGuard = 26,//
          CoreOfficials = 27,
          AppointCoreOffical = 28,//
          RemoveCoreOffical = 29,//
          ReSignCoreOffical = 30,//
          Officials = 34,
          Tokens = 37,
          SaveLeagueOrderStatu = 38,//
          Stipend = 39,
          ImperialHarem = 40,
          AppointLeagueConcubines = 41,//
          UppdateLeagueConcubines =42,
          RemoveHarem = 44,//
          Allowance = 46,
          AppointCore = 47,//
          AppointGuard = 48,//
          AppointHarem = 49,//
          UpdateHarem = 50,
          ChangeUnionName = 51,
          Announce = 66,
          PlunderWar = 69



      }
      public static unsafe void GetLeagueOpt(this ServerSockets.Packet stream, out ActionID Action, out uint ID
            ,out uint UID, out uint Index, out string[] texts)
        {
            Action = (ActionID)stream.ReadUInt16();
            ID = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            Index = stream.ReadUInt32();
            texts = stream.ReadStringList();

        }

        public static unsafe ServerSockets.Packet LeagueOptCreate(this ServerSockets.Packet stream, ActionID Action, uint ID, 
            uint UID, uint Index, params string[] texts)
        {
            stream.InitWriter();

            stream.Write((ushort)Action);
            stream.Write(ID);
            stream.Write(UID);
            stream.Write(Index);
            if (texts != null)
                stream.Write(texts);
            else
                stream.ZeroFill(2);

            stream.Finalize(GamePackets.LeagueOpt);
            return stream;
        }
        /// <summary>First client supplied text, or null when the packet carries none.</summary>
        private static string FirstText(string[] texts)
        {
            if (texts == null || texts.Length == 0)
                return null;
            return texts[0];
        }

        /// <summary>Paging offset that can never be negative or overflow.</summary>
        private static int PageOffset(long page, int max)
        {
            if (page < 0) page = 0;
            return (int)Math.Min(page * max, int.MaxValue / 2);
        }

        /// <summary>Ranks the emperor may hand out through the appoint actions (never Emperor or Member).</summary>
        private static bool IsCoreOfficialRank(uint rank)
        {
            return rank >= (uint)Role.Instance.Union.Member.MilitaryRanks.LeftPre && rank <= (uint)Role.Instance.Union.Member.MilitaryRanks.CO_Gen
                && Enum.IsDefined(typeof(Role.Instance.Union.Member.MilitaryRanks), rank);
        }
        private static bool IsHaremRank(uint rank)
        {
            return rank == (uint)Role.Instance.Union.Member.MilitaryRanks.Empress || rank == (uint)Role.Instance.Union.Member.MilitaryRanks.SeniorConc
                || rank == (uint)Role.Instance.Union.Member.MilitaryRanks.Conc;
        }

        /// <summary>
        /// Token states for the Kingdom Token dialog (MsgLeagueTokens / CMsgLeagueOrderStatusPB):
        /// param1 = token, param2 = times used, param3 = cooldown seconds left.
        /// </summary>
        static void SendTokens(Client.GameClient user, ServerSockets.Packet stream)
        {
            var union = user.Player.MyUnion;
            uint[] ids = { 101, Role.Instance.Union.SummonTokenID, 103, 106, Role.Instance.Union.ShiftTokenID, Role.Instance.Union.StipendTokenID, 206 };
            MsgLeagueTokens.GroupToken GroupTokens = new MsgLeagueTokens.GroupToken();
            GroupTokens.Tokens = new MsgLeagueTokens.Token[ids.Length];
            GroupTokens.Count = (uint)ids.Length;
            for (int i = 0; i < ids.Length; i++)
                GroupTokens.Tokens[i] = new MsgLeagueTokens.Token() { param1 = ids[i], param3 = union.TokenCooldownLeft(ids[i]) };
            user.Send(stream.CreateLeagueTokens(GroupTokens));
        }

        [PacketAttribute(GamePackets.LeagueOpt)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            ActionID Action;
            uint ID; 
            uint UID;
            uint Index;
            string[] texts;
         
            stream.GetLeagueOpt(out Action, out ID, out UID, out Index, out texts);
            Role.Instance.Union.EnsureBound(user);
            if (MsgTournaments.KingdomWarHandlers.HandleLeagueOpt(user, stream, (ushort)Action, ID, UID, Index)) return; // Plunder War actions 60/64/69/71/78
            //51 change name
            switch (Action)
            {
                case ActionID.TransferLeader:
                    {
                        // client: ID = 0, Index = own union id, UID = the hero who takes over (it asked
                        // "Abdicate the leadership of the Union to %s?" itself)
                        if (!user.Player.IsUnionEmperror)
                            break;
                        var union = user.Player.MyUnion;
                        if (Index != 0 && Index != union.UID)
                            break;
                        if (UID == 0 || UID == user.Player.UID)
                            break;
                        var target = union.FindMember(UID);
                        if (target == null)
                        {
                            user.CreateBoxDialog("This hero is not a member of your " + (union.IsKingdom == 1 ? "Kingdom" : "Union") + ".");
                            break;
                        }
                        if (!union.CanLead(target))
                        {
                            user.CreateBoxDialog("You can only abdicate to the leader of a member guild or to a member without a guild.");
                            break;
                        }
                        union.TransferLeader(stream, user.Player.UnionMemeber, target);
                        break;
                    }
                case ActionID.ChangeUnionName:
                    {
                        var name = FirstText(texts);
                        if (name == null) break;
                        if (!user.Player.IsUnionEmperror)
                        {
                            user.CreateBoxDialog("Failed to change the name. You`re not the Union Leader.");
                            break;
                        }
                        if (name.Length > 15)
                        {
                            user.CreateBoxDialog("This name contains illegal characters or forbidden words. Please rename the Union.");
                            break;
                        }
                        if (Role.Instance.Union.CheckName(name))
                        {
                            uint cost = Role.Instance.Union.RenameCost;
                            if (user.Player.ConquerPoints < cost)
                            {
                                user.CreateBoxDialog("You don`t have enough CPs to change the Union`s name.");
                                break;
                            }
                            if (user.Player.InUnion && user.Player.IsUnionEmperror)
                            {
                                if (cost > 0)
                                    user.Player.ConquerPoints -= cost;
                                user.Player.MyUnion.Name = name;
                                foreach (var player in Database.Server.GamePoll.Values)
                                    if (player.Player.InUnion && player.Player.MyUnion.UID == user.Player.MyUnion.UID)
                                        user.Player.MyUnion.SendMyInfo(stream, player);
                                user.Send(stream.LeagueOptCreate(ActionID.ChangeUnionName, 0, 0, user.Player.MyUnion.UID, ""));
                            }
                        }
                        else
                            user.CreateBoxDialog("This name contains illegal characters or forbidden words. Please rename the Union.");
                        break;
                    }
                case ActionID.NewUnionTitle:
                    {
                        // the Emperor names his Kingdom ("Enter a valid Kingdom Title. (5 letters)"); the reply shows the
                        // proclamation STR_ZHI_COUNTRY_NAME with the date taken from Index (yyyymmdd)
                        var title = MsgGuildProces.SafeText(FirstText(texts), 16);
                        if (string.IsNullOrEmpty(title))
                            break;
                        title = title.Trim();
                        if (!user.Player.IsUnionEmperror || user.Player.MyUnion.IsKingdom != 1)
                        {
                            user.CreateBoxDialog("Failed to change the title. You`re not the Emperor.");
                            break;
                        }
                        if (title.Length == 0 || title.Length > 5 || global::Conquer6609.AnsiText.Value.GetByteCount(title) > 9
                            || !Program.NameStrCheck(title, false) || title.Contains(" "))
                        {
                            user.CreateBoxDialog("Enter a valid Kingdom Title. (5 letters)");
                            break;
                        }
                        var union = user.Player.MyUnion;
                        union.Title = title;
                        uint date = Role.Instance.Union.DayStamp(DateTime.Now);
                        user.Send(stream.LeagueOptCreate(ActionID.NewUnionTitle, 0, 0, date, title));
                        union.SendUnionSysMessage(user.Player.Name + " declared himself Emperor of `" + title + "`!");
                        break;
                    }
                case ActionID.MyUnion:
                    {
                        // the client keeps its union id from this (also sent by Union.SendMyInfo / after create / pledge)
                        if (user.Player.InUnion)
                            user.Send(stream.LeagueOptCreate(ActionID.MyUnion, user.Player.MyUnion.UID, 0, user.Player.MyUnion.UID, ""));
                        break;
                    }
                case ActionID.KingdomTitleGui:
                    {
                        // opens the kingdom title dialog for the Emperor (the title itself comes back as NewUnionTitle)
                        if (user.Player.IsUnionEmperror && user.Player.MyUnion.IsKingdom == 1)
                            user.Send(stream.LeagueOptCreate(ActionID.KingdomTitleGui, user.Player.MyUnion.UID, 0, 0, ""));
                        else
                            user.CreateBoxDialog("Failed to change the title. You`re not the Emperor.");
                        break;
                    }
                case ActionID.RecruitDeclaration:
                    {
                        // shown to heroes looking for a Union to join (LeagueAllegianceList)
                        if (!user.Player.IsUnionEmperror)
                            break;
                        var text = MsgGuildProces.SafeText(FirstText(texts) ?? "", 254);
                        text = text.Trim();
                        if (text.Length > 0 && !Program.NameStrCheck(text, false))
                        {
                            user.SendSysMesage("The recruit declaration contains illegal characters. Please release again.");
                            break;
                        }
                        user.Player.MyUnion.RecruitDeclaration = text.Length == 0 ? "None" : text;
                        user.SendSysMesage("The recruit declaration has been released.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                        break;
                    }
                case ActionID.ReSignPalaceGuard:
                    {
                        if (!user.Player.InUnion)
                            break;
                        var me = user.Player.UnionMemeber;
                        if (user.Player.MyUnion.Resign(me, true))
                            user.Send(stream.LeagueOptCreate(ActionID.ReSignPalaceGuard, 0, me.UID, 0, me.Name));
                        break;
                    }
                case ActionID.ReSignCoreOffical:
                    {
                        if (!user.Player.InUnion)
                            break;
                        var me = user.Player.UnionMemeber;
                        uint oldRank = (uint)me.Rank;
                        if (user.Player.MyUnion.Resign(me, false))
                        {
                            user.Send(stream.LeagueOptCreate(ActionID.ReSignCoreOffical, 0, me.UID, oldRank, me.Name));
                            user.SendSysMesage("Successfully resigned!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                        }
                        break;
                    }
                case ActionID.Allowance:
                    {
                        if (user.Player.InUnion)
                            user.Player.MyUnion.ClaimAllowance(user, stream);
                        break;
                    }
                case ActionID.Stipend:
                    {
                        // client: ID = own union id, Index = 1 free pack / 2 War Exploits for 3 packs / 3 CPs for 5 packs
                        if (!user.Player.InUnion)
                            break;
                        if (ID != 0 && ID != user.Player.MyUnion.UID)
                            break;
                        user.Player.MyUnion.ClaimStipend(user, stream, Index);
                        break;
                    }
                case ActionID.SaveLeagueOrderStatu:
                    {
                        // issue a token from the Kingdom Token dialog: ID = token (103 / 205 / 206; 102 and 204 come as MsgLeagueToken)
                        if (!user.Player.InUnion)
                            break;
                        if (ID == Role.Instance.Union.StipendTokenID)
                        {
                            if (!user.Player.IsUnionEmperror)
                            {
                                user.CreateBoxDialog("Only the Emperor can issue a Stipend Token.");
                                break;
                            }
                            if (user.Player.MyUnion.IssueStipend(user, stream))
                                SendTokens(user, stream);
                        }
                        break;
                    }
                case ActionID.Quit:
                    {
                        if (user.Player.MyGuild != null && user.Player.InUnion)
                        {
                            if (user.Player.MyGuild.UnionID == user.Player.MyUnion.UID)
                                break;
                            if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                                break;
                        }
                        else if(user.Player.InUnion)
                        {
                            if (user.Player.IsUnionEmperror)
                            {
                                user.CreateBoxDialog("Please abdicate the leadership before quitting the Union.");
                                break;
                            }
                          
                            user.Send(stream.LeagueOptCreate(ActionID.MyUnion, 0, 0, user.Player.MyUnion.UID, ""));
                            user.Player.MyUnion.Quit(stream, user.Player.UnionMemeber);
                    
                            //to do quit.
                        }
                        break;
                    }
                case ActionID.GuildQuit:
                    {
                        if (user.Player.MyGuildMember == null || user.Player.MyGuild == null)
                            break;
                        if (user.Player.MyGuildMember.Rank != Role.Flags.GuildMemberRank.GuildLeader)
                            break;
                        if (user.Player.MyUnion == null)
                            break;
                        // A guild leader may only take his own guild out of the union; the emperor's guild stays.
                        if (ID != user.Player.MyGuild.Info.GuildID)
                            break;
                        if (user.Player.MyGuild.Members.ContainsKey(user.Player.MyUnion.EmperrorUID))
                            break;
                        Role.Instance.Guild guild;
                        if (user.Player.MyUnion.Guilds.TryGetValue(ID, out guild))
                        {
                            user.Send(stream.LeagueOptCreate(ActionID.MyUnion, 0, 0, user.Player.MyUnion.UID, ""));
                            user.Player.MyUnion.ExpelGuild(stream, guild);
                        }


                        break;
                    }
                case ActionID.ExpelGuild:
                    {
                        if (user.Player.IsUnionEmperror)
                        {

                            Role.Instance.Guild guild;
                            if (user.Player.MyUnion.Guilds.TryGetValue(ID, out guild))
                            {
                                // The emperor cannot expel his own guild.
                                if (guild.Members.ContainsKey(user.Player.UID))
                                    break;
                                user.Player.MyUnion.ExpelGuild(stream, guild);
                            }
                        }
                        break;
                    }
                case ActionID.ExpelMember:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member;
                            if (user.Player.MyUnion.Members.TryGetValue(Index, out Member))
                            {
                                if (Member.UID == user.Player.UID || Member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                    break;
                                user.Player.MyUnion.Quit(stream, Member);
                            }
                        }
                        break;
                    }
                case ActionID.Pledge:
                    {
                        if (user.Player.InUnion == false)
                        {
                            if (user.Player.MyGuild == null)
                            {
                                  Role.Instance.Union union;
                                  if (Role.Instance.Union.UnionPoll.TryGetValue(Index, out union))
                                  {
                                      union.AddOtherMember(stream, user);
                                  }
                            }
                        }
                        break;
                    }
                case ActionID.GuildPledge:
                    {
                        if (user.Player.InUnion == false && user.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader
                            && user.Player.MyGuild != null)
                        {
                            Role.Instance.Union union;
                            if (Role.Instance.Union.UnionPoll.TryGetValue(Index, out union))
                            {
                                union.AddGuild(stream, user.Player.MyGuild);
                            }
                        }
                        break;
                    }
                case ActionID.List:
                    {
                        if (user.Player.InUnion == false)
                        {
                            const int max = 3;
                            var array = Role.Instance.Union.GetOrderyUnions;
                            int offset = PageOffset(Index, max);
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeagueAllegianceListCreate((uint)array.Length, (ushort)Index, (ushort)count, 1);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var element = array[x + offset];
                                stream.AddItemLeagueAllegianceList(element.Treasury, element.UID, element.GoldBrick, element.Name, element.Emperor, element.RecruitDeclaration,element.IsKingdom);
                            }
                            user.Send(stream.LeagueAllegianceListFinalize());
                          
                            //for(int x = 0; x< 100; x++)
                            //    user.Send(stream.LeagueOptCreate((ActionID)x, array[0].UID, array[0].UID, 4, array[0].Name));
                        }
                        break;
                    }

                case ActionID.ImperialGuards:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;
                            var array = user.Player.MyUnion.ImperialGuards.Values.ToArray();

                            int offset = PageOffset((long)Index - 1, max);
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeaguePalaceGuardsListCreate((byte)Index, (byte)array.Length);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var element = array[x + offset];

                                if (element.IsOnline)
                                    stream.AddItemLeaguePalaceGuardsList((uint)element.Owner.Player.BattlePower, element.Mesh, element.Exploits, (uint)element.NobilityRank, element.UID, element.Level, element.Class, 1, element.Name);
                                else
                                    stream.AddItemLeaguePalaceGuardsList(0, element.Mesh, element.Exploits, (uint)element.NobilityRank, element.UID, element.Level, element.Class, 0, element.Name);
                            }


                            user.Send(stream.LeaguePalaceGuardsListFinalize());

                        }
                        break;
                    }
               
                case ActionID.RemovePalaceGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member;
                            if (user.Player.MyUnion.ImperialGuards.TryGetValue(UID, out Member))
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));
                            }
                        }
                        break;
                    }
               
                case ActionID.RemoveCoreOffical:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member = user.Player.MyUnion.GetOfficialMemberByUID(UID);
                            if (Member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));

                                
                            }
                        }
                        break;
                    }
                case ActionID.RemoveHarem:
                    {

                        if (user.Player.IsUnionEmperror)
                        {
                            Role.Instance.Union.Member Member = user.Player.MyUnion.GetImperialHaremMemberByUID(UID);
                            if (Member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(Member, Role.Instance.Union.Member.MilitaryRanks.Member, true);
                                user.Send(stream.LeagueOptCreate(Action, 0, UID, 0, Member.Name));


                            }
                        }
                        break;
                    }
                case ActionID.ImperialHarem:
                    {
                       
                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.GetImperialHarem;
                            MsgLeagueConcubines.LeagueImperialHarem harem = new MsgLeagueConcubines.LeagueImperialHarem();
                            harem.type = (byte)1;
                            harem.ImperialHarems = new MsgLeagueConcubines.ImperialHarem[array.Length];
                            int index_s = 0;
                            int index_c = 0;
                            for (int x = 0; x < array.Length; x++)
                            {
                                harem.ImperialHarems[x] = new MsgLeagueConcubines.ImperialHarem(array[x]);
                                if (harem.ImperialHarems[x].Rank == 401)
                                {
                                    harem.ImperialHarems[x].Rank = (uint)(4011 + index_s);
                                    index_s += 1;
                                }
                                if (harem.ImperialHarems[x].Rank == 402)
                                {
                                    harem.ImperialHarems[x].Rank = (uint)(4021 + index_c);
                                    index_c += 1;
                                }
                            }
                            user.Send(stream.CreateLeagueConcubines(harem));
                        }
                        break;
                    }
               
                case ActionID.AppointGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            if (user.Player.MyUnion.ImperialGuards.Count == 20)
                            {
                                user.CreateBoxDialog("Sorry the imperial guards list is full.");
                                break;
                            }
                            // Appoint guard: always the imperial guard rank (whatever the client sent), never on the emperor.
                            Index = (uint)Role.Instance.Union.Member.MilitaryRanks.Imprl_Guard;
                            var member = user.Player.MyUnion.GetMemberByName(FirstText(texts));
                            if (member == null || member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                user.Send(stream.LeagueOptCreate(ActionID.AppointGuard, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.AppointImperialGuard:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            if (user.Player.MyUnion.ImperialGuards.Count == 20)
                            {
                                user.CreateBoxDialog("Sorry the imperial guards list is full.");
                                break;
                            }
                            Index = (uint)Role.Instance.Union.Member.MilitaryRanks.Imprl_Guard;
                            var member = user.Player.MyUnion.GetMemberByName(FirstText(texts));
                            if (member == null || member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                user.Send(stream.LeagueOptCreate(ActionID.AppointImperialGuard, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.AppointLeagueConcubines:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var member = user.Player.MyUnion.GetMemberByName(FirstText(texts));
                            if (member == null || member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {

                                if (Index >= 4010 && Index <= 4020 )
                                    Index = 401;
                                if (Index >= 4020 && Index <= 4030)
                                    Index = 402;
                                // Concubine appointments only hand out harem ranks.
                                if (!IsHaremRank(Index))
                                    break;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Empress)
                                    user.Player.MyUnion.CheckUpHarem(member, (Role.Instance.Union.Member.MilitaryRanks)Index);


                                bool CanAdd = true;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.SeniorConc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 2;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Conc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 7;
                                if (CanAdd)
                                {
                                    user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);
                                    user.Send(stream.LeagueOptCreate(ActionID.UppdateLeagueConcubines, 0, member.UID, (uint)member.Rank, member.Name));
                                
                                }
                                else
                                {
                                    user.CreateBoxDialog("Sorry the " + ((Role.Instance.Union.Member.MilitaryRanks)Index).ToString() + " list is full.");
                                }
                            }
                        }
                        break;
                    }
                case ActionID.AppointHarem:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            // Client sub-indices map onto the harem ranks exactly as for AppointLeagueConcubines.
                            if (Index >= 4010 && Index <= 4020)
                                Index = 401;
                            if (Index >= 4020 && Index <= 4030)
                                Index = 402;
                            // Harem appointments only hand out harem ranks.
                            if (!IsHaremRank(Index))
                                break;
                            var member = user.Player.MyUnion.GetMemberByName(FirstText(texts));
                            if (member == null || member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                                return;
                            if (member != null)
                            {
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Empress)
                                    user.Player.MyUnion.CheckUpHarem(member, (Role.Instance.Union.Member.MilitaryRanks)Index);
                                
                                bool CanAdd = true;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.SeniorConc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 2;
                                if ((Role.Instance.Union.Member.MilitaryRanks)Index == Role.Instance.Union.Member.MilitaryRanks.Conc)
                                    CanAdd = user.Player.MyUnion.GetImperialHaremCount((Role.Instance.Union.Member.MilitaryRanks)Index) < 7;
                                if (CanAdd)
                                {
                                    user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);

                                    user.Send(stream.LeagueOptCreate(ActionID.UpdateHarem, 0, member.UID, (uint)member.Rank, member.Name));
                                }
                                else
                                {
                                    user.CreateBoxDialog("Sorry the " + ((Role.Instance.Union.Member.MilitaryRanks)Index).ToString() + " list is full.");
                                }
                            }
                        }
                        break;
                    }
                case ActionID.AppointCoreOffical:
                case ActionID.AppointCore:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                           // Core officials: only the official ranks between LeftPre and CO_Gen.
                           if (!IsCoreOfficialRank(Index))
                               break;
                           var member = user.Player.MyUnion.GetMemberByName(FirstText(texts));
                           if (member == null || member.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                               return;
                            if (member != null)
                            {
                                user.Player.MyUnion.UpdateUnioOfficials(member, (Role.Instance.Union.Member.MilitaryRanks)Index, false);

                                user.Send(stream.LeagueOptCreate(Action, 0, member.UID, (uint)member.Rank, member.Name));
                            }
                        }
                        break;
                    }
                case ActionID.CoreOfficials:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.UnionOfficials;
                            stream.LeagueImperialCourtListCreate(MsgLeagueImperialCourtList.ActionID.CoreOfficials, (byte)array.Length);
                            foreach (var member in array)
                            {
                                if (member.IsOnline)
                                    stream.AddItemLeagueImperialCourtList((uint)member.Owner.Player.BattlePower, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                                else
                                    stream.AddItemLeagueImperialCourtList(0, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                            }

                            user.Send(stream.LeagueImperialCourtListFinalize());
                        }
                        break;
                    }
                case ActionID.Tokens:
                    {
                        if (user.Player.InUnion)
                            SendTokens(user, stream);
                        break;
                    }
                case ActionID.Officials:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = user.Player.MyUnion.UnionOfficials;
                            stream.LeagueImperialCourtListCreate(MsgLeagueImperialCourtList.ActionID.Officials, (byte)array.Length);




                            //var emeperor = user.Player.MyUnion.GetEmperor();
                         //   stream.AddItemLeagueImperialCourtList(0, user.Player.MyUnion.e, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                            foreach (var member in array)
                            {
                                if (member.IsOnline)
                                    stream.AddItemLeagueImperialCourtList((uint)member.Owner.Player.BattlePower, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                                else
                                    stream.AddItemLeagueImperialCourtList(0, member.Mesh, member.Exploits, (uint)member.NobilityRank, member.UID, member.Level, member.Class, (ushort)member.Rank, 1, member.Name);
                            }

                            user.Send(stream.LeagueImperialCourtListFinalize());
                        }
                        break;
                    }
                case ActionID.UnionRank:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = MsgInterServer.Instance.Union.InfoUnions.Values.ToArray();
                            const int max = 10;

                            int offset = PageOffset(Index, max);
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));
                            stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)array.Length, (ushort)Index, 1, (ushort)count);
                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueRank(entity.ServerID, entity.GoldBricks, entity.Name, entity.LeaderName);
                            }
                            user.Send(stream.LeagueRankFinalize());
                        }
                        break;
                    }
                case ActionID.KingdomRank:
                    {
                        if (user.Player.InUnion)
                        {
                            var array = Role.Instance.Union.Top3Unions;
                            stream.LeagueRankCreate(0, 3, 1, (uint)array.Length, 0);
                            for (int x = 0; x < array.Length; x++)
                                stream.AddItemLeagueRank(array[x].GoldBrick, array[x].Name, array[x].Emperor, 0);
                            user.Send(stream.LeagueRankFinalize());
                        }
                        break;
                    }
                case ActionID.ViewOthers:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;


                            var array = user.Player.MyUnion.GetOnlineMembers;

                            int offset = PageOffset(Index, max);
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeagueMemListCreate(0, 0, (uint)array.Length, (ushort)count, 1);

                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueMemList(entity.Exploits, entity.Class, entity.Level, entity.Mesh, entity.IsOnline, entity.Name, (uint)Role.Instance.Union.Member.GetRank(entity.Rank), (uint)entity.NobilityRank, entity.UID, (ushort)(entity.IsOnline ? entity.Owner.Player.BattlePower : 0));
                            }
                            user.Send(stream.LeagueMemListFinalize());
                        }
                        break;
                    }
                  
                case ActionID.Members:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 10;

                            Role.Instance.Guild Guild;
                            // Member lists only for guilds of the player's own union.
                            if (user.Player.MyUnion.Guilds.TryGetValue(ID, out Guild))
                            {
                                var array = Guild.GetOnlineMembers.Where(p => p.UnionMem != null).ToArray();
                             
                                int offset = PageOffset(Index, max);
                                int count = Math.Min(max, Math.Max(0, array.Length - offset));
                                var guildLeader = Guild.GetGuildLeader;

                                stream.LeagueMemListCreate(Guild.Info.GuildID, guildLeader != null ? guildLeader.UID : 0, (uint)array.Length, (ushort)count, 1);

                                for (byte x = 0; x < count; x++)
                                {
                                    if (x + offset >= array.Length)
                                        break;
                                    var entity = array[x + offset].UnionMem;
                                    stream.AddItemLeagueMemList(entity.Exploits, entity.Class, entity.Level, entity.Mesh, entity.IsOnline, entity.Name, (uint)Role.Instance.Union.Member.GetRank(entity.Rank), (uint)entity.NobilityRank, entity.UID, (ushort)(entity.IsOnline ? entity.Owner.Player.BattlePower : 0));

                                }
                                user.Send(stream.LeagueMemListFinalize());
                            }
                        }
                        break;
                    }
                case ActionID.SynList:
                    {
                        if (user.Player.InUnion)
                        {
                            const int max = 15;

                            var array = user.Player.MyUnion.GetOrderGuilds;

                            int offset = PageOffset(Index, max);
                            int count = Math.Min(max, Math.Max(0, array.Length - offset));

                            stream.LeagueSynListCreate(array.Length, (ushort)(count), 1);//1???

                            for (byte x = 0; x < count; x++)
                            {
                                if (x + offset >= array.Length)
                                    break;
                                var entity = array[x + offset];
                                stream.AddItemLeagueSynList(entity.Info.GuildID, (ulong)entity.Info.SilverFund, (ushort)entity.Info.MembersCount, (ushort)entity.Info.Level, entity.GuildName, entity.Info.LeaderName);
                            }
                            user.Send(stream.LeagueSynListFinalize());
                        }
                        break;
                    }
                case ActionID.Announce:
                    {
                        if (user.Player.IsUnionEmperror)
                        {
                            var str = MsgGuildProces.SafeText(FirstText(texts), MsgGuildProces.MaxBulletinLength);
                            if (Program.NameStrCheck(str, false))
                            {
                                user.Player.MyUnion.Bulletin = str;
                            }
                            else
                            {
#if Arabic
                                     user.SendSysMesage("Invalid Announce charasters.");
#else
                                user.SendSysMesage("Invalid Announce charasters.");
#endif
                            }
                        }
                        break;
                    }
                case ActionID.Info:
                    {
                        if (user.Player.InUnion)
                            user.Player.MyUnion.SendInfo(stream, user);
                        break;
                    }
                case ActionID.Create:
                    {
                        var name = FirstText(texts);
                        if (name == null || user.Player.MyGuildMember == null) break;
                       // user.Player.MyUnion = Role.Instance.Union.Create(stream, user, name);
                       // user.Player.MyUnion.AddGuild(stream, user.Player.MyGuild);

                        if (user.Player.InUnion == false)
                        {
                            if (user.Player.MyGuild != null)
                            {
                                if (user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader && user.Player.MyGuild.Info.Level >= 9)
                                {
                                    if (user.Player.MyGuild.Info.ConquerPointFund >= 3000)
                                    {
                                    
                                        if (Role.Instance.Union.CheckName(name))
                                        {
                                            user.Player.MyGuild.Info.ConquerPointFund -= 3000;
                                            user.Player.MyUnion = Role.Instance.Union.Create(stream,user, name);
                                            user.Player.MyUnion.AddGuild(stream, user.Player.MyGuild);
                                        }
                                        else
                                           user.CreateBoxDialog("There is a Union Already with this Name i'm sorry.");
                                    }
                                    else
                                        user.CreateBoxDialog("Sorry, you don`t have 3000 CPs in guild to create a Union.");
                                }
                                else
                                    user.CreateBoxDialog("Sorry, you aren't a guild leader or your guild isn't LVL 9");
                            }
                            else
                                user.CreateBoxDialog("You Aren't in a Guild Yet.");
                        }
                        else
                            user.CreateBoxDialog("You Are Already in a Union");
                        break;
                    }
            }
        }
    }
}
