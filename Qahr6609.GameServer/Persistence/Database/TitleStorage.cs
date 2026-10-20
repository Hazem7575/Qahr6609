using Conquer6609.Game.MsgTournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Database
{
    public class TitleStorage
    {

        public uint ID = 0;
        public uint SubID = 0;
        public string Name = "";
        public uint Score = 0;

        public static Dictionary<uint, TitleStorage> Titles = new Dictionary<uint, TitleStorage>();

        public static void LoadDBInformation()
        {

            string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "title_type.txt");
            foreach (var bas_line in baseText)
            {
                var line = bas_line.Split(' ');
                TitleStorage title = new TitleStorage();
                title.ID = uint.Parse(line[0]);
                title.SubID = uint.Parse(line[1]);
                title.Name = line[2];
                title.Score = uint.Parse(line[7]);
                Titles.Add(title.ID, title);
            }
        }

        public static void CheckUpUser(Client.GameClient user, ServerSockets.Packet stream)
        {
            //if (user.Player.Name.Contains("[GM]") || user.ProjectManager == true)
            //{
               
            //    foreach (var itemm in Database.TitleStorage.Titles.Values)
            //        user.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)itemm.ID, stream);
            //    return;
            //}


            #region WingsofInfernal - Legendary
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Legendary, stream);

                    }
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] == null || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length == 0 || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID != user.Player.UID)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Legendary, stream);
                }
            }
            #endregion
            #region Peerless
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Peerless))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[1].UID == user.Player.UID)
                    {

                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Peerless, stream);

                    }
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Peerless))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] == null || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length == 0 || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[1].UID != user.Player.UID)
                {

                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Peerless, stream);
                }
            }
            #endregion
            #region Outstanding
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Outstanding))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[2].UID == user.Player.UID)
                    {

                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Outstanding, stream);

                    }
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Outstanding))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] == null || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length == 0 || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[2].UID != user.Player.UID)
                {

                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Outstanding, stream);
                }
            }
            #endregion
            #region Expert
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Expert))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[4].UID == user.Player.UID)
                    {

                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Expert, stream);

                    }
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Expert))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] == null || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length == 0 || Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[4].UID != user.Player.UID)
                {

                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Expert, stream);
                }
            }
            #endregion
            #region Vip
            #region  EarthKnight
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight))
            {
                if (user.Player.VipLevel >= 0)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight))
            {
                if (user.Player.VipLevel >= 0)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
                }
            }
            #endregion

            #region  EmeraldGlowWing
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.EmeraldGlowWing))
            {
                if (user.Player.VipLevel >= 3)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EmeraldGlowWing, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.EmeraldGlowWing))
            {
                if (user.Player.VipLevel < 3)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EmeraldGlowWing, stream);
                }
            }
            #endregion
            #region  OrangeGlowWings
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.OrangeGlowWings))
            {
                if (user.Player.VipLevel >= 4)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.OrangeGlowWings, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.OrangeGlowWings))
            {
                if (user.Player.VipLevel < 4)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.OrangeGlowWings, stream);
                }
            }
            #endregion
            #region  FlameGlowWings
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FlameGlowWings))
            {
                if (user.Player.VipLevel >= 5)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FlameGlowWings, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FlameGlowWings))
            {
                if (user.Player.VipLevel < 5)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FlameGlowWings, stream);
                }
            }
            #endregion
            #region  BrightGlowWings
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.BrightGlowWings))
            {
                if (user.Player.VipLevel >= 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.BrightGlowWings, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.BrightGlowWings))
            {
                if (user.Player.VipLevel < 6)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.BrightGlowWings, stream);
                }
            }
            #endregion
            #endregion
            #region  WingsofPlanet
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofPlanet))
            {
                if (user.Player.Level == 140)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofPlanet, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofPlanet))
            {
                if (user.Player.Level < 140)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofPlanet, stream);
                }
            }

            #endregion
            #region  FairyWings
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings))
            {
                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MRConquerHost) || user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MSConquerHostess))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.PeerlessBeauty, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SuperIdol, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings))
            {
                if (!user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MRConquerHost) || user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.MSConquerHostess))
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.PeerlessBeauty, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SuperIdol, stream);
                }
            }
            #endregion
            #region  WingsofRomance
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofRomance))
            {
                if (user.MyWardrobe.Contain(194405) && user.MyWardrobe.Contain(193445) && user.MyWardrobe.Contain(194395))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofRomance, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofRomance))
            {
                if (!(user.MyWardrobe.Contain(194405) && user.MyWardrobe.Contain(193445) && user.MyWardrobe.Contain(194395)))
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofRomance, stream);
                }
            }
            #endregion
            #region  VioletLightning
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning))
            {
                if (user.PrestigeLevel >= 216)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning))
            {
                if (user.PrestigeLevel < 216)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning, stream);
                }
            }

            #endregion
            #region  VioletCloudWing

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing))
            {
                if (user.PrestigeLevel >= 324)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing))
            {
                if (user.PrestigeLevel < 324)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing, stream);
                }
            }
            #endregion
            #region  MoonlightWings
            if (CoatStorage.AmountStarMount(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings))
            {
                if (CoatStorage.AmountStarMount(user, 5) < 1)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);
                }
            }

            #endregion
            #region  BigFan
            if (CoatStorage.AmountStarMount(user, 4) >= 1 || CoatStorage.AmountStarMount(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.BigFan, stream);
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EuroCollector, stream);
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.BigFan))
            {
                if (CoatStorage.AmountStarMount(user, 4) < 1 || CoatStorage.AmountStarMount(user, 5) < 1)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.BigFan, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EuroCollector, stream);
                }
            }

            #endregion
            #region  StarlightWings
            if (CoatStorage.AmountStarGarments(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings))
            {
                if (CoatStorage.AmountStarGarments(user, 5) < 1)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);
                }
            }
            #endregion
            #region Beauty
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Beauty))
            {
                if (Role.Core.IsGirl(user.Player.Body))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Beauty, stream);
                }
            }
            else if (!Role.Core.IsGirl(user.Player.Body))
            {
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Beauty, stream);
                }
            }
            #endregion
            #region Wise
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Wise))
            {
                if (Role.Core.IsBoy(user.Player.Body))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Wise, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Handsome, stream);
                }
            }
            else if (!Role.Core.IsBoy(user.Player.Body))
            {
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Wise, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Handsome, stream);
                }
            }
            #endregion
            #region KingTeam
            if (user.Player.Teampk)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.KingTeam, stream);
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.DominatorTeam, stream);
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.PowerTeam, stream);
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EliteTeam, stream);
            }
            else
            {
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.KingTeam, stream);
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.DominatorTeam, stream);
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.PowerTeam, stream);
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EliteTeam, stream);
            }
            #endregion
            #region Prestige - Scholar -
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Superman))
            {
                if (user.MyPrestigePoints >= 600000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);

                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.HonoredHero, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Superman))
            {
                if (user.MyPrestigePoints < 600000)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.HonoredHero, stream);
                }
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);

                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionMaster, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionIdol, stream);
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.MyPrestigePoints < 400000)
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionMaster, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionIdol, stream);
                }
            }


            #endregion
            #region Supreme -Invincible
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Supreme))
            {
                Game.MsgServer.MsgGameItem item;
                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out item))
                {
                    if (item.ITEM_ID == 2100094)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Supreme, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Invincible, stream);
                    }
                }
            }
            if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Supreme))
            {
                Game.MsgServer.MsgGameItem item;
                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out item))
                {
                    if (item.ITEM_ID != 2100094)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Supreme, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Invincible, stream);
                    }
                }
            #endregion
            #region RadiantWings - Victor
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Victor))
                {
                    if (user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Victor))
                {
                    if (!(user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                    }
                }
                #endregion
            #region  Conqueror
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror))
                {
                    if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                    {
                        if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
                            user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror))
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID != user.Player.GuildID)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);

                    }
                }
                #endregion
            #region  VioletCloudWing

                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Great))
                {
                    if (user.PrestigeLevel >= 648)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Great, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Great))
                {
                    if (user.PrestigeLevel < 648)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Great, stream);
                    }
                }
                #endregion
            #region VIPTitles
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight))
                {
                    if (user.Player.VipLevel >= 1 && user.Player.VipLevel <= 4)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight))
                {
                    if (user.Player.VipLevel < 1 || user.Player.VipLevel > 4)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight))
                {
                    if (user.Player.VipLevel == 5 || user.Player.VipLevel == 6)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight))
                {
                    if (user.Player.VipLevel < 5 || user.Player.VipLevel > 6)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Paladin))
                {
                    if (user.Player.VipLevel == 7)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Paladin, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Paladin))
                {
                    if (user.Player.VipLevel != 7)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Paladin, stream);
                    }
                }
                #endregion
            #region Monkeys
                ////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.LoveMadness))
                {
                    if (user.MyWardrobe.Contain(194875) && user.MyWardrobe.Contain(200613) && user.MyWardrobe.Contain(200614) && user.Player.Body / 1000 == 1)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MostRadiant, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.LoveMadness))
                {
                    if (!user.MyWardrobe.Contain(194875) || !user.MyWardrobe.Contain(200613) || !user.MyWardrobe.Contain(200614) || user.Player.Body / 1000 != 1)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MostRadiant, stream);
                    }
                }
                ////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy))
                {
                    if (user.MyWardrobe.Contain(194875) && user.MyWardrobe.Contain(200613) && user.MyWardrobe.Contain(200614) && user.Player.Body / 1000 == 2)
                    {
                        if (Role.Core.IsGirl(user.Player.Body))
                        {
                            user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy, stream);
                        }
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy))
                {
                    if (!user.MyWardrobe.Contain(194875) || !user.MyWardrobe.Contain(200613) || !user.MyWardrobe.Contain(200614) || user.Player.Body / 1000 != 2)
                    {
                        if (!Role.Core.IsGirl(user.Player.Body))
                        {
                            user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy, stream);
                        }
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy))
                {
                    if (user.MyWardrobe.Contain(194875) && user.MyWardrobe.Contain(200613) && user.MyWardrobe.Contain(200614) && user.Player.Body / 1000 == 2)
                    {
                        if (Role.Core.IsBoy(user.Player.Body))
                        {
                            user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy, stream);
                            user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LoveMadness, stream);

                        }
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy))
                {
                    if (!user.MyWardrobe.Contain(194875) || !user.MyWardrobe.Contain(200613) || !user.MyWardrobe.Contain(200614) || user.Player.Body / 1000 != 2)
                    {
                        if (!Role.Core.IsBoy(user.Player.Body))
                        {
                            user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SweetFairy, stream);
                            user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LoveMadness, stream);
                        }
                    }
                }
                ////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate))
                {
                    if (user.MyWardrobe.Contain(200595))
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate))
                {
                    if (!user.MyWardrobe.Contain(200595))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SuccessortoLege))
                {
                    if (user.MyWardrobe.Contain(200596))
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SuccessortoLege, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SuccessortoLege))
                {
                    if (!user.MyWardrobe.Contain(200596))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SuccessortoLege, stream);
                    }
                }
                /////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Lovejoker))
                {
                    if (user.MyWardrobe.Contain(194875) || user.MyWardrobe.Contain(194865))
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Lovejoker, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Lovejoker))
                {
                    if (!user.MyWardrobe.Contain(194875) || user.MyWardrobe.Contain(194865))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Lovejoker, stream);
                    }
                }
                //////////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SirenSong))
                {
                    if (user.MyWardrobe.Contain(189675))
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SirenSong, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SirenSong))
                {
                    if (!user.MyWardrobe.Contain(189675))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SirenSong, stream);
                    }
                }
                ////
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider))
                {
                    if (user.MyWardrobe.CollectedRandomMonkey())
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider))
                {
                    if (!user.MyWardrobe.CollectedRandomMonkey())
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider))
                {
                    if (user.MyWardrobe.CollectedAllMonkeyTypes())
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider))
                {
                    if (!user.MyWardrobe.CollectedAllMonkeyTypes())
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider))
                {
                    if (user.MyWardrobe.CollectedSolarMonkey())
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MostAttractive, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider))
                {
                    if (!user.MyWardrobe.CollectedSolarMonkey())
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MostAttractive, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider))
                {
                    if (user.MyWardrobe.CollectedLunarMonkey())
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider))
                {
                    if (!user.MyWardrobe.CollectedLunarMonkey())
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider, stream);
                    }
                }
                #endregion
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FashionIcon))
                {
                    if (user.Player.FlowerRank == 1)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionIcon, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FashionIcon))
                {
                    if (user.Player.FlowerRank != 1)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FashionIcon, stream);
                    }
                }

                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar))
                {
                    if (user.Player.Achievement.Score() >= 300)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar, stream);
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fairy, stream);

                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar))
                {
                    if (user.Player.Achievement.Score() < 300)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fairy, stream);
                    }
                }

                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Talent))
                {
                    if (user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000)
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Talent))
                {
                    if (user.Player.MyJiangHu == null || user.Player.MyJiangHu.Inner_Strength < 80000)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
                    }
                }
                if (CoatStorage.AmountStarGarments(user, 4) >= 5 || CoatStorage.AmountStarGarments(user, 5) >= 1)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist))
                {
                    if (CoatStorage.AmountStarGarments(user, 4) < 5 || CoatStorage.AmountStarGarments(user, 5) < 1)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
                    }
                }

                if (!(user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SlamDunkLeader, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SlamDunkLeader, stream);
                }

                if (CoatStorage.AmountStarMount(user, 4) >= 5)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Goddess, stream);
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser))
                {
                    if (CoatStorage.AmountStarMount(user, 4) < 5)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Goddess, stream);
                    }
                }

                if (user.Player.InUnion && user.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
                }

                if (CoatStorage.AmountStarMount(user, 4) >= 5)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser))
                {
                    if (CoatStorage.AmountStarMount(user, 4) < 5)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
                    }
                }
                if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster))
                {
                    if (user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
                    }
                }
                if (user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster))
                {
                    if (user.Player.MyChi == null || user.Player.MyChi.AllScore() < 1500)
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
                    }
                }
                //if (user.Player.Name.Contains("[GM]") || user.ProjectManager == true)
                //{
                //    var Wing = Database.TitleStorage.Titles.Values.Where(x => x.ID >= 4001 && x.ID < 6017).ToArray();

                //    foreach (var itemm in Wing)
                //        user.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)itemm.ID, stream);
                //    //foreach (var itemm in Database.TitleStorage.Titles.Values)
                //    //    user.Player.AddSpecialTitle((Game.MsgServer.MsgTitleStorage.TitleType)itemm.ID, stream);
                //}


            }
        }

    }
}

