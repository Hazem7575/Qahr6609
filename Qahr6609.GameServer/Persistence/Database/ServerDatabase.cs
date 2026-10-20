using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer6609.Game.MsgServer;
using System.Windows.Forms;
using Conquer6609.Game.Conquer6609_Work;

namespace Conquer6609.Database
{
    public class ServerDatabase
    {
        public static void ResetingEveryDay(Client.GameClient client)
        {
            try
            {
                if (DateTime.Now.DayOfYear != client.Player.Day)
                {
                    client.Player.UseChiToken = 0;
                    client.Player.MiningAttempts = 140;
                    client.Player.ChangeEpicTrojan = client.Player.ChangeArrayEpicTrojan =
                        client.Player.ChangeMr_MirrorEpicTrojan = client.Player.ChangeGeneralPakEpicTrojan  = 0;
                    client.Player.CanChangeEpicMaterial = client.Player.CanChangeArrayEpicMaterial  =
                        client.Player.CanChangeMr_MirrorEpicMaterial = client.Player.CanChangGeneralPakMaterial = 1;
                    client.Player.ConquerPointsLimitaion = 0;
                    client.Player.TodayChampionPoints = 0;
                    client.MyExchangeShop.Reset();
                    if (client.Player.DailyMonth == 0)
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    if (client.Player.DailyMonth != DateTime.Now.Month)
                    {
                        client.Player.DailySignUpRewards = 0;
                        client.Player.DailySignUpDays = 0;
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    }
                    if (client.Player.MyJiangHu != null)
                    {
                        client.Player.MyJiangHu.FreeCourse = 100000;
                        client.Player.MyJiangHu.FreeTimeToday = 0; 
                        client.Player.MyJiangHu.RoundBuyPoints = 0;
                    }
                    if (client.Activeness != null)
                    {
                        client.Activeness.ResetDaily();
                    }
                    client.Player.MisShoot = client.Player.HitShoot = 0;
                    client.Player.ArenaDeads = client.Player.ArenaKills = 0;
                    client.Player.MisSBShoot = client.Player.HitSBShoot = 0;
                    client.Player.ArenaSBDeads = client.Player.ArenaSBKills = 0;
                    client.Player.TowerOfMysterychallenge = 3;
                    client.Player.TOMChallengeToday = 0;
                    client.Player.TowerOfMysteryChallengeFlag = 0;
                    client.Player.TOMSelectChallengeToday = 0;
                    client.Player.ClaimTowerAmulets = 0;
                    client.Player.TOMClaimTeamReward = 0;
                    client.Player.TOMRefreshReward = 0;
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.GiantGhasomKill = 0;
                    client.Player.GiantGhasomKillClaimed = false;
                    client.Player.ClaimedBCPToday = false;
                    client.Player.FateFruitUsed = 0;
                    client.Player.WrongAnswers = new int[5] { 3, 3, 3, 3, 3 };
                    client.Player.OpenHousePack = 0;
                    client.Player.AnswerdToday = 0;
                    client.Player.DbTry = false;
                    client.Player.MSConquer = false;
                    client.Player.MRConquer = false;
                    client.Player.LotteryEntries = 0;
                    client.Player.BDExp = 0;
                    client.Player.ExpBallUsed = 0;
                    client.Player.ExpFruit = 0;
                    client.Player.SwordMaster = 0;
                    client.Player.TeratoDragon = 0;
                    client.Player.PurbleBanshee = 0;
                    client.Player.TCCaptainTimes = 0;
                    client.DemonExterminator.FinishToday = 0;
                    #region  EPICQUESTS
                    client.Player.helpladytime = 0;
                    client.Player.BattleFieldPoints = 0;
                    #endregion
                    client.Player.EpicQuestChance = 0;
                    if (client.Player.MyChi != null && DateTime.Now.DayOfYear > client.Player.Day)
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + Math.Min(((DateTime.Now.DayOfYear - client.Player.Day) * 300), 4000);
                    else
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;
                    client.Player.Flowers.FreeFlowers = 1;
                    foreach (var flower in client.Player.Flowers)
                        flower.Amount2day = 0;
                    // late sign-in chances are monthly now (VIP table, see MsgSignIn.LateSignInLeft); not refilled daily
                    if (client.Player.Level >= 90)
                    {
                        client.Player.Enilghten = CalculateEnlighten(client.Player);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                        }
                    }
                    client.Player.BuyKingdomDeeds = 0;
                    client.Player.QuestGUI.RemoveQuest(35024);
                    client.Player.QuestGUI.RemoveQuest(35007);
                    client.Player.QuestGUI.RemoveQuest(35025);
                    client.Player.QuestGUI.RemoveQuest(35028);
                    client.Player.QuestGUI.RemoveQuest(35034);

                    //---- reset Quests
                    client.Player.QuestGUI.RemoveQuest(6390);
                    client.Player.QuestGUI.RemoveQuest(6329);
                    client.Player.QuestGUI.RemoveQuest(6245);
                    client.Player.QuestGUI.RemoveQuest(6049);
                    client.Player.QuestGUI.RemoveQuest(6366);
                    client.Player.QuestGUI.RemoveQuest(6014);
                    client.Player.QuestGUI.RemoveQuest(2375);
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                        = client.Player.DailyMagnoliaItemId
                        = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                    //
                    client.Player.Day = DateTime.Now.DayOfYear;
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, object> SaveLocks = new System.Collections.Concurrent.ConcurrentDictionary<uint, object>();
        static int SavesInFlight;
        /// <summary>Number of SaveClient calls currently running (used to wait for them on shutdown).</summary>
        public static int SavesRunning { get { return System.Threading.Volatile.Read(ref SavesInFlight); } }

        static readonly Func<uint, object> NewSaveLock = u => new object();
        static object SaveLockOf(uint uid)
        {
            return SaveLocks.GetOrAdd(uid, NewSaveLock);
        }

        /// <summary>Saves the character now, on the calling thread. Prefer QueueSave/QueueUrgentSave from packet
        /// handlers: they run the same save on the player-save thread.</summary>
        public static void SaveClient(Client.GameClient client)
        {
            if (client == null || client.Player == null)
                return;
            // Saves of the same character can come from the save queue and from packet handlers;
            // serialize them so they never interleave on the same files.
            object saveLock = SaveLockOf(client.Player.UID);
            System.Threading.Interlocked.Increment(ref SavesInFlight);
            try
            {
                lock (saveLock)
                    SaveClientCore(client);
            }
            finally
            {
                System.Threading.Interlocked.Decrement(ref SavesInFlight);
            }
        }
        static void SaveClientCore(Client.GameClient client)
        {

            try
            {
                SaveState saveState = StateOf(client);
                // Read once: a periodic save that sees the Disconnect flag set halfway must not act as the logout
                // save (the logout save queued by the disconnect runs after this one and is the full, final one).
                bool finalSave = (client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect;
                string iniPath = Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini";
                PlayerIniWriter write = new PlayerIniWriter(iniPath, saveState, client.Player.UID, finalSave);

                if ((client.ClientFlag & Client.ServerFlag.LoginFull) != Client.ServerFlag.LoginFull)
                {

                    if (client.Map != null)
                        client.Map.Denquer(client);

                }

                if (HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID != 0 || client.Player.DynamicID != 0)
                {
                    if (client.Socket != null && client.Socket.Alive == false)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 301;
                        client.Player.Y = 278;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                {
                    if (client.Player.Map == 1036 || client.InQualifier() || client.Player.DynamicID == client.Player.UID)
                    {
                        if (client.Socket != null && client.Socket.Alive == false)
                        {
                            client.Player.Map = 1002;
                            client.Player.X = 301;
                            client.Player.Y = 278;
                        }
                    }
                    if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                        || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                    || client.Player.Map == 1806
                        || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                        || client.Player.Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
                || client.Player.Map == 1768
                || client.Player.Map == Game.MsgTournaments.MsgFootball.MapID
                || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507
                 || client.Player.Map == 1801 || client.Player.Map == 1780 || client.Player.Map == 1779 || client.Player.Map == 3071 || client.Player.Map == 1068

                        || client.Player.Map == 3830 || client.Player.Map == 3831 || client.Player.Map == 3832 || client.Player.Map == 3834
                        || client.Player.Map == 3826 || client.Player.Map == 3827 || client.Player.Map == 3828 || client.Player.Map == 3829
                        || client.Player.Map == 3833 || client.Player.Map == 3825

                        || Game.MsgTournaments.MsgSchedules.PowerArena.InPowerArena(client)
                        || client.Player.Map == 4745 || client.Player.Map == 5051

                        || client.Player.Map == 10088 || client.Player.Map == 10089 || client.Player.Map == 10090
                        || client.Player.Map == 44455 || client.Player.Map == 44456 || client.Player.Map == 44457
                         || client.Player.Map == 44460 || client.Player.Map == 44461 || client.Player.Map == 44462 || client.Player.Map == 44463
)
                    {

                        client.Player.Map = 1002;
                        client.Player.X = 301;
                        client.Player.Y = 278;
                    }
                  
                    if (client.Player.Map == Game.MsgTournaments.MsgFootball.MapID)
                    {
                        var tourn = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgFootball;
                        if (tourn != null)
                        {

                            if (client.Player.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                            {
                                client.Player.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                                tourn.FinishRound();
                            }
                        }
                    }
                    if (client.Player.Map == 4000 || client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 301;
                        client.Player.Y = 278;
                    }
                }

                if (!client.FullLoading)
                    return;

                write.Write<uint>("Character", "UID", client.Player.UID);
                write.Write<ushort>("Character", "Body", client.Player.Body);
                write.Write<ushort>("Character", "Face", client.Player.Face);
                write.WriteString("Character", "Name", client.Player.Name);
                write.WriteString("Character", "Spouse", client.Player.Spouse);
                write.Write<byte>("Character", "Class", client.Player.Class);
                write.Write<byte>("Character", "FirstClass", client.Player.FirstClass);
                write.Write<byte>("Character", "SecoundeClass", client.Player.SecoundeClass);
                try
                {
                    write.Write<bool>("Character", "CPBoundPackToday", client.Player.ClaimedBCPToday);
                    write.Write<byte>("Character", "FateFruitUsed", client.Player.FateFruitUsed);
                    write.Write<byte>("Character", "CPBoundPack", client.Player.CPBoundPack);
                    write.Write<uint>("Character", "ConquerPointsLimitaion", client.Player.ConquerPointsLimitaion);
                    write.Write<uint>("Character", "AnswerdToday", client.Player.AnswerdToday);
                    write.Write<byte>("Character", "ConquerPointDropLimitLayer", client.Player.ConquerPointDropLimitLayer);
                    write.Write<uint>("Character", "ExchangeNormalAvaliability", client.Player.ExchangeNormalAvaliability);
                    write.Write<uint>("Character", "ExchangehighAvaliability", client.Player.ExchangehighAvaliability);
                    write.Write<uint>("Character", "GiantGhasomKill", client.Player.GiantGhasomKill);
                    write.Write<uint>("Character", "MiningAttempts", client.Player.MiningAttempts);
                    write.Write<bool>("Character", "GiantGhasomKillClaimed", client.Player.GiantGhasomKillClaimed);
                    if (client.Player.ConquerPointDropLimitLayer > 1)
                    {
                        write.Write<long>("Character", "ConquerPointsDropStamp", client.Player.ConquerPointsDropStamp.ToBinary());
                    }
                    //write.Write<string>("Character", "MacAddress", client.PcLogin.MacAddress);
                    if (client.Player.WrongAnswers != null)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            write.Write<int>("Character", "WrongAnswers" + x.ToString(), client.Player.WrongAnswers[x]);
                        }
                    }
                }
                catch
                {
                    MyConsole.WriteLine("Error Writing New Values.");
                }
                write.Write<uint>("Character", "DailySpiritBeadCount", client.Player.DailySpiritBeadCount);
                write.Write<ushort>("Character", "Avatar", client.Player.Avatar);
                write.Write<uint>("Character", "Map", client.Player.Map);
                write.Write<ushort>("Character", "X", client.Player.X);
                write.Write<ushort>("Character", "Y", client.Player.Y);
                write.Write<byte>("Character", "ChargeChi", client.Player.ChargeChi);
                write.Write<byte>("Character", "ChargeJaing", client.Player.ChargeJaing);
                //write.Write<uint>("Character", "Merchant", client.Player.Merchant);
                //write.Write<long>("Character", "MerchantApplicationEnd", client.Player.MerchantApplicationEnd.ToBinary());
                write.Write<ulong>("Character", "BankCps", client.ConquerPointsSave);
                write.Write<uint>("Character", "PMap", client.Player.PMap);
                write.Write<ushort>("Character", "PMapX", client.Player.PMapX);
                write.Write<ushort>("Character", "PMapY", client.Player.PMapY);

                write.Write<ushort>("Character", "Agility", client.Player.Agility);
                write.Write<ushort>("Character", "Strength", client.Player.Strength);
                write.Write<ushort>("Character", "Vitaliti", client.Player.Vitality);
                write.Write<ushort>("Character", "Spirit", client.Player.Spirit);
                write.Write<ushort>("Character", "Atributes", client.Player.Atributes);
                write.Write<bool>("Character", "IsBannedChat", client.Player.IsBannedChat);
                write.Write<bool>("Character", "PermenantBannedChat", client.Player.PermenantBannedChat);
                write.Write<long>("Character", "BannedChatStamp", client.Player.BannedChatStamp.ToBinary());
                write.Write<byte>("Character", "Reborn", client.Player.Reborn);
                write.Write<ushort>("Character", "Level", client.Player.Level);
                write.Write<ushort>("Character", "Haire", client.Player.Hair);
                write.Write<ulong>("Character", "Experience", client.Player.Experience);
                write.Write<int>("Character", "MinHitPoints", client.Player.HitPoints);
                write.Write<ushort>("Character", "MinMana", client.Player.Mana);

                write.Write<uint>("Character", "ConquerPoints", client.Player.ConquerPoints);
                write.Write<int>("Character", "BoundConquerPoints", client.Player.BoundConquerPoints);
                write.Write<long>("Character", "Money", client.Player.Money);
                write.Write<uint>("Character", "VirtutePoints", client.Player.VirtutePoints);

                write.Write<ushort>("Character", "PkPoints", client.Player.PKPoints);
                write.Write<uint>("Character", "QuizPoints", client.Player.QuizPoints);
                write.Write<bool>("Character", "Teampk", client.Player.Teampk);

                write.Write<ushort>("Character", "Enilghten", client.Player.Enilghten);
                write.Write<ushort>("Character", "EnlightenReceive", client.Player.EnlightenReceive);
                write.Write<ulong>("Character", "DailySignUpDays", client.Player.DailySignUpDays);
                write.Write<ushort>("Character", "LateSignIn", client.Player.LateSignIn);
                write.Write<byte>("Character", "DailyMonth", client.Player.DailyMonth);
                write.Write<byte>("Character", "DailySignUpRewards", client.Player.DailySignUpRewards);

                write.Write<byte>("Character", "VipLevel", client.Player.VipLevel);
                write.Write<long>("Character", "VipTime", client.Player.ExpireVip.Ticks);
                client.Player.Achievement.Save(client.Achievement);
                write.WriteString("Character", "Achivement", client.Achievement.ToString());
                write.Write<long>("Character", "WHMoney", client.Player.WHMoney);
                write.Write<uint>("Character", "BlessTime", client.Player.BlessTime);

                write.Write<uint>("Character", "SpouseUID", client.Player.SpouseUID);

                write.Write<int>("Character", "HeavenBlessing", client.Player.HeavenBlessing);
                write.Write<uint>("Character", "LostTimeBlessing", client.Player.HeavenBlessTime.Value);

                write.Write<uint>("Character", "HuntingBlessing", client.Player.HuntingBlessing);
                write.Write<uint>("Character", "OnlineTrainingPoints", client.Player.OnlineTrainingPoints);
                write.Write<long>("Character", "JoinOnflineTG", client.Player.JoinOnflineTG.Ticks);

                write.Write<int>("Character", "Day", client.Player.Day);
                write.Write<byte>("Character", "BDExp", client.Player.BDExp);

                write.Write<uint>("Character", "SwordMaster", client.Player.SwordMaster);
                write.Write<uint>("Character", "TeratoDragon", client.Player.TeratoDragon);
                write.Write<uint>("Character", "PurbleBanshee", client.Player.PurbleBanshee);

                write.Write<uint>("Character", "DExpTime", client.Player.DExpTime);
                write.Write<uint>("Character", "RateExp", client.Player.RateExp);
                write.Write<byte>("Character", "ExpBallUsed", client.Player.ExpBallUsed);
                write.Write<byte>("Character", "ExpFruitUsed", client.Player.ExpFruit);

                write.WriteString("Character", "SubProfInfo", client.Player.SubClass.ToString());

                write.WriteString("Character", "Dragon", client.Player.MyChi.Dragon.ToString());
                write.WriteString("Character", "Pheonix", client.Player.MyChi.Phoenix.ToString());
                write.WriteString("Character", "Turtle", client.Player.MyChi.Turtle.ToString());
                write.WriteString("Character", "Tiger", client.Player.MyChi.Tiger.ToString());
                write.Write<int>("Character", "ChiPoints", client.Player.MyChi.ChiPoints);
                if (client.Player.MyChi.DragonTime != 0)
                {
                    write.Write<long>("Character", "DragonTime", client.Player.MyChi.DragonTime);
                    write.WriteString("Character", "RetreatedDragon", ChiTable.PowersToString(client.Player.MyChi.DragonPowers));
                }
                else
                    write.Write<long>("Character", "DragonTime", 0);//abandoned lock must not come back after a restart
                if (client.Player.MyChi.PhoenixTime != 0)
                {
                    write.Write<long>("Character", "PhoenixTime", client.Player.MyChi.PhoenixTime);
                    write.WriteString("Character", "RetreatedPhoenix", ChiTable.PowersToString(client.Player.MyChi.PhoenixPowers));
                }
                else
                    write.Write<long>("Character", "PhoenixTime", 0);//abandoned lock must not come back after a restart
                bool wroteTurtleKey = false, wroteTigerKey = false;
                if (client.Player.MyChi.TurtleTime != 0)
                {
                    wroteTurtleKey = true;
                    write.Write<long>("Character", "TurtleTime", client.Player.MyChi.TurtleTime);
                    write.WriteString("Character", "RetreatedTurtle", ChiTable.PowersToString(client.Player.MyChi.TurtlePowers));
                }
                else
                    write.Write<long>("Character", "TurtleTime", 0);
                if (client.Player.MyChi.TigerTime != 0)
                {
                    wroteTigerKey = true;
                    write.Write<long>("Character", "TigerTime", client.Player.MyChi.TigerTime);
                    write.WriteString("Character", "RetreatedTiger", ChiTable.PowersToString(client.Player.MyChi.TigerPowers));
                }
                else
                    write.Write<long>("Character", "TigerTime", 0);
                // Older builds stored the Tiger/Turtle retreat powers under each other's key; this marker tells
                // ChiTable.Load that the keys of this file are in the corrected layout. Only set it when both keys
                // were rewritten by this save; otherwise a stale old-layout key would be read with the new layout.
                // (a lock that was not rewritten now gets time 0 above, so its stale key is never read again)
                if ((wroteTurtleKey && wroteTigerKey) || client.Player.MyChi.TurtleTime == 0 || client.Player.MyChi.TigerTime == 0)
                    write.Write<byte>("Character", ChiTable.RetreatKeysMarker, 1);

                write.WriteString("Character", "Flowers", client.Player.Flowers.ToString());
                write.Write<ulong>("Character", "DonationNobility", client.Player.Nobility.Donation);

                write.Write<uint>("Character", "GuildID", client.Player.GuildID);
                write.Write<ushort>("Character", "GuildRank", (ushort)client.Player.GuildRank);
                if (client.Player.MyGuildMember != null)
                {
                    client.Player.MyGuildMember.LastLogin = DateTime.Now.Ticks;
                    write.Write<uint>("Character", "CpsDonate", client.Player.MyGuildMember.CpsDonate);
                    write.Write<long>("Character", "MoneyDonate", client.Player.MyGuildMember.MoneyDonate);
                    write.Write<uint>("Character", "PkDonation", client.Player.MyGuildMember.PkDonation);
                    write.Write<long>("Character", "LastLogin", client.Player.MyGuildMember.LastLogin);

                    write.Write<uint>("Character", "CTF_Exploits", client.Player.MyGuildMember.CTF_Exploits);
                    write.Write<uint>("Character", "CTF_RCPS", client.Player.MyGuildMember.RewardConquerPoints);
                    write.Write<uint>("Character", "CTF_RM", client.Player.MyGuildMember.RewardMoney);
                    write.Write<byte>("Character", "CTF_R", client.Player.MyGuildMember.CTF_Claimed);
                }
                if (client.Player.MyClan != null)
                {
                    write.Write<uint>("Character", "ClanID", client.Player.MyClan.ID);
                    write.Write<ushort>("Character", "ClanRank", client.Player.ClanRank);
                    if (client.Player.MyClanMember != null)
                        write.Write<uint>("Character", "ClanDonation", client.Player.MyClanMember.Donation);
                }
                // union membership (restored at login by Union.BindPlayer; guild members follow their guild)
                if (client.Player.InUnion)
                {
                    var unionMember = client.Player.UnionMemeber;
                    write.Write<uint>("Character", "UnionUID", client.Player.MyUnion.UID);
                    write.Write<uint>("Character", "UnionRank", (uint)unionMember.Rank);
                    write.Write<uint>("Character", "UnionExploits", unionMember.Exploits);
                    write.Write<uint>("Character", "Treasury", unionMember.MyTreasury);
                    write.Write<uint>("Character", "UnionAllowanceDay", unionMember.AllowanceDay);
                    write.Write<uint>("Character", "UnionStipend", unionMember.StipendClaimed);
                }
                else
                {
                    write.Write<uint>("Character", "UnionUID", 0);
                    write.Write<uint>("Character", "UnionRank", 0);
                }


                write.Write<uint>("Character", "KingDomExploits", client.Player.KingDomExploits);
                write.Write<byte>("Character", "FRL", client.Player.FirstRebornLevel);
                write.Write<byte>("Character", "SRL", client.Player.SecoundeRebornLevel);
                write.Write<bool>("Character", "Reincanation", client.Player.Reincarnation);
                write.Write<byte>("Character", "LotteryEntries", client.Player.LotteryEntries);
                // a drawn but not accepted lottery prize survives the logout (re-shown at login)
                write.WriteString("Character", "LotteryPrize", Database.LotteryTable.SavePrize(client.Player));
                write.Write<bool>("Character", "DbTry", client.Player.DbTry);
                write.Write<bool>("Character", "MSConquer", client.Player.MSConquer);
                write.Write<bool>("Character", "MRConquer", client.Player.MRConquer);
                write.WriteString("Character", "DemonEx", client.DemonExterminator.ToString());
                write.WriteString("Character", "PkName", client.Player.MyKillerName);
                write.Write<uint>("Character", "PkUID", client.Player.MyKillerUID);
                write.Write<int>("Character", "Cursed", client.Player.CursedTimer);
                write.WriteString("Character", "HeroRewards", client.HeroRewards.ToString());
                write.WriteString("Character", "Activeness", client.Activeness.ToString());
                write.Write<uint>("Character", "AparenceType", client.Player.AparenceType);

                write.Write<uint>("Character", "HitShoot", client.Player.HitShoot);
                write.Write<uint>("Character", "MisShoot", client.Player.MisShoot);
                write.Write<uint>("Character", "ArenaDeads", client.Player.ArenaDeads);
                write.Write<uint>("Character", "ArenaKills", client.Player.ArenaKills);

                write.Write<uint>("Character", "HitSBShoot", client.Player.HitSBShoot);
                write.Write<uint>("Character", "MisSBShoot", client.Player.MisSBShoot);
                write.Write<uint>("Character", "ArenaSBDeads", client.Player.ArenaSBDeads);
                write.Write<uint>("Character", "ArenaSBKills", client.Player.ArenaSBKills);

                write.Write<uint>("Character", "EpicQuestChance", client.Player.EpicQuestChance);
                write.Write<uint>("Character", "TKills", client.Player.TournamentKills);
                write.Write<uint>("Character", "OnlineMinutes", client.Player.OnlineMinutes);

                write.Write<uint>("Character", "HistoryChampionPoints", client.Player.HistoryChampionPoints);
                write.Write<uint>("Character", "TodayChampionPoints", client.Player.TodayChampionPoints);

                write.Write<uint>("Character", "ChampionPoints", client.Player.ChampionPoints);
                write.Write<uint>("Character", "DailySpiritBeadItem", client.Player.DailySpiritBeadItem);
                write.WriteString("Character", "SpecialTitles", GetSpecialTitles(client));
                write.WriteString("Character", "SecurityPass", GetSecurityPassword(client));
                write.Write<byte>("Character", "TCT", (byte)client.Player.TCCaptainTimes);
                write.Write<uint>("Character", "RacePoints", client.Player.RacePoints);
                write.Write<uint>("Character", "SteedRaceBest", client.Player.SteedRaceBestTime);
                write.Write<ushort>("Character", "NameEditCount", client.Player.NameEditCount);
                write.Write<uint>("Character", "ClaimStateGift", (uint)client.Player.MainFlag);
                write.Write<uint>("Character", "enervant", client.Player.AtiveQuestApe);
                write.Write<ushort>("Character", "InventorySashCount", client.Player.InventorySashCount);

                write.Write<ushort>("Character", "CountryID", client.Player.CountryID);
                write.Write<uint>("Character", "MyFootBallPoints", client.Player.MyFootBallPoints);
                write.Write<uint>("Character", "ExpProtection", client.Player.ExpProtection);
                write.Write<uint>("Character", "PrestigePoints", client.MyPrestigePoints);
                write.Write<uint>("Character", "BanCount", client.BanCount);
                write.Write<uint>("Character", "SpecialWingID", client.Player.SpecialWingID);

                write.Write<byte>("Character", "BuyKingdomDeeds", client.Player.BuyKingdomDeeds);
                write.Write<uint>("Character", "KingDomDeeds", client.Player.KingDomDeeds);
                //  write.Write<uint>("Character", "KingDomGold", client.Player.KingDomGold);
                write.Write<ulong>("Character", "RechargeProgress", (ulong)client.Player.RechargeProgress);

                write.WriteString("Character", "ExchangeShop", client.MyExchangeShop.ToString());

                write.Write<ushort>("Character", "ExtraAtributes", client.Player.ExtraAtributes);

                write.Write<byte>("Character", "OpenHousePack", client.Player.OpenHousePack);


                write.Write<byte>("Character", "ClaimTowerAmulets", client.Player.ClaimTowerAmulets);
                write.Write<byte>("Character", "TOMClaimTeamReward", client.Player.TOMClaimTeamReward);
                write.Write<byte>("Character", "MyTowerOfMysteryLayer", client.Player.MyTowerOfMysteryLayer);
                write.Write<byte>("Character", "MyTowerOfMysteryLayerElite", client.Player.MyTowerOfMysteryLayerElite);
                write.Write<byte>("Character", "TowerOfMysterychallenge", client.Player.TowerOfMysterychallenge);
                write.Write<uint>("Character", "TowerOfMysteryChallengeFlag", client.Player.TowerOfMysteryChallengeFlag);
                write.Write<byte>("Character", "TOMSelectChallengeToday", client.Player.TOMSelectChallengeToday);
                write.Write<byte>("Character", "TOMChallengeToday", client.Player.TOMChallengeToday);
                write.Write<uint>("Character", "TOMRefreshReward", client.Player.TOMRefreshReward);
                write.Write<byte>("Character", "TOM_Reward", (byte)client.Player.TOM_Reward);
                write.Write<long>("Character", "JPAStamp", client.Player.JoinPowerArenaStamp.Ticks);

                write.WriteString("Character", "EpicTrojan", client.Player.SaveEpicTrojan());
                write.Write<int>("Character", "GiveFlowersToPerformer", client.Player.GiveFlowersToPerformer);
                write.Write<byte>("Character", "UseChiToken", client.Player.UseChiToken);
                write.Write<long>("Character", "CanChangeWindWalkerFree", client.Player.CanChangeWindWalkerFree.Ticks);
                #region CQUESTS
                write.Write<byte>("Character", "HelpLady", client.Player.helpladytime);
                write.Write<uint>("Character", "VotePoints", client.Player.VotePoints);
                write.Write<uint>("Character", "Battlefieldpoints", client.Player.BattleFieldPoints);
                //     write.Write<int>("Character", "GiveFlowersToPerformer", client.Player.GiveFlowersToPerformer);
                #endregion
              
                SaveClientItems(client);
                SaveClientSpells(client);
                SaveClientProfs(client);
                RoleQuests.Save(client);
                Halo_Flag.Save(client);//Halo_Flag
                Role.Instance.House.Save(client);

                if (finalSave)
                {
                    // Only this session's entry: a stale save of an older session must never remove the newer one.
                    ((ICollection<KeyValuePair<uint, Client.GameClient>>)Database.Server.GamePoll).Remove(
                        new KeyValuePair<uint, Client.GameClient>(client.Player.UID, client));
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            // [Storage] copy the ini just written into its MySQL row (read now under the save lock, written by a MirrorQueue
            // worker). Here, not in SaveClient: the queued saves (ExecuteSaveItem) call SaveClientCore directly.
            if (client.Player != null)
                Storage.PlayerStorage.OnSaved(client.Player.UID, client.Player.Name);

        }
        public static void LoadCharacter(Client.GameClient client, uint UID)
        {
            // same per-character lock as SaveClient: a load never reads files while a save replaces them
            lock (SaveLockOf(UID))
            {
                // A save of this character requested before this login (e.g. the previous session's logout save
                // still waiting on the player-save thread) is written first, so the load always sees it.
                FlushPendingSaves(UID, client);
                // [Storage] PlayerReadSource=Db/Verify: the players row is a read overlay of Users\uid.ini while loading
                Storage.PlayerStorage.BeforeLoad(UID);
                try
                {
                    LoadCharacterCore(client, UID);
                }
                finally
                {
                    Storage.PlayerStorage.AfterLoad(UID);
                }
            }
        }
        static void LoadCharacterCore(Client.GameClient client, uint UID)
        {
            client.Player.UID = UID;
            // Validate the binary save files before touching any state. A character whose files are damaged
            // (and have no valid .bak) is not loaded at all: loading it empty would overwrite the real data.
            PlayerBinaryFiles binaryFiles;
            try
            {
                binaryFiles = ResolvePlayerFiles(UID);
            }
            catch (InvalidDataException)
            {
                // The caller (MsgLoginClient) tells the player why and disconnects.
                MyConsole.WriteLine("Login refused for " + UID + ": damaged save files (see Security_Logs).");
                throw;
            }
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
            client.Player.Body = reader.ReadUInt16("Character", "Body", 1002);
            client.Player.Face = reader.ReadUInt16("Character", "Face", 0);
            client.Player.Name = reader.ReadString("Character", "Name", "None");
            client.Player.Spouse = reader.ReadString("Character", "Spouse", "None");
            client.Player.Class = reader.ReadByte("Character", "Class", 0);
            client.Player.FirstClass = reader.ReadByte("Character", "FirstClass", 0);
            client.Player.ChargeJaing = reader.ReadByte("Character", "ChargeJaing", 0);
            client.Player.ChargeChi = reader.ReadByte("Character", "ChargeChi", 0);
            client.Player.SecoundeClass = reader.ReadByte("Character", "SecoundeClass", 0);
            //client.Player.MerchantApplicationEnd = DateTime.FromBinary(reader.ReadInt64("Character", "MerchantApplicationEnd", 0));
            //client.Player.Merchant = reader.ReadUInt32("Character", "Merchant", 0);
            client.Player.Avatar = reader.ReadUInt16("Character", "Avatar", 0);
            client.Player.Map = reader.ReadUInt32("Character", "Map", 1002);
            client.Player.X = reader.ReadUInt16("Character", "X", 248);
            client.Player.Y = reader.ReadUInt16("Character", "Y", 238);

            for (int x = 0; x < 5; x++)
            {
                client.Player.WrongAnswers[x] = reader.ReadInt32("Character", "WrongAnswers" + x.ToString(), 0);
            }
            client.Player.PermenantBannedChat = reader.ReadBool("Character", "PermenantBannedChat", false);
            client.Player.IsBannedChat = reader.ReadBool("Character", "IsBannedChat", false);
            client.Player.BannedChatStamp = DateTime.FromBinary(reader.ReadInt64("Character", "BannedChatStamp", 0));
            client.Player.PMap = reader.ReadUInt32("Character", "PMap", 1002);
            client.Player.PMapX = reader.ReadUInt16("Character", "PMapX", 300);
            client.Player.PMapY = reader.ReadUInt16("Character", "PMapY", 300);
            client.ConquerPointsSave = reader.ReadUInt64("Character", "BankCps ", 0);
            client.Player.Agility = reader.ReadUInt16("Character", "Agility", 0);
            client.Player.Strength = reader.ReadUInt16("Character", "Strength", 0);
            client.Player.Spirit = reader.ReadUInt16("Character", "Spirit", 0);
            client.Player.Vitality = reader.ReadUInt16("Character", "Vitaliti", 0);
            client.Player.Atributes = reader.ReadUInt16("Character", "Atributes", 0);
            client.Player.Reborn = reader.ReadByte("Character", "Reborn", 0);
            client.Player.Level = reader.ReadUInt16("Character", "Level", 0);
            client.Player.Hair = reader.ReadUInt16("Character", "Haire", 0);
            client.Player.Experience = (ulong)reader.ReadInt64("Character", "Experience", 0);
            client.Player.HitPoints = reader.ReadInt32("Character", "MinHitPoints", 0);
            client.Player.Mana = reader.ReadUInt16("Character", "MinMana", 0);
            client.Player.ConquerPoints = reader.ReadUInt32("Character", "ConquerPoints", 0);
            client.Player.BoundConquerPoints = reader.ReadInt32("Character", "BoundConquerPoints", 0);
            client.Player.Money = reader.ReadInt64("Character", "Money", 0);
            client.Player.VirtutePoints = reader.ReadUInt32("Character", "VirtutePoints", 0);
            client.Player.PKPoints = reader.ReadUInt16("Character", "PkPoints", 0);
            client.Player.QuizPoints = reader.ReadUInt32("Character", "QuizPoints", 0);
            client.Player.Teampk = reader.ReadBool("Character", "Teampk", false);
            client.Player.Enilghten = reader.ReadUInt16("Character", "Enilghten", 0);
            client.Player.EnlightenReceive = reader.ReadUInt16("Character", "EnlightenReceive", 0);
            client.Player.LateSignIn = reader.ReadUInt16("Character", "LateSignIn", 0);
            client.Player.DailySignUpDays = reader.ReadUInt64("Character", "DailySignUpDays", 0);
            client.Player.DailyMonth = reader.ReadByte("Character", "DailyMonth", 0);
            client.Player.DailySignUpRewards = reader.ReadByte("Character", "DailySignUpRewards", 0);
            client.Player.VipLevel = reader.ReadByte("Character", "VipLevel", 0);
            client.Player.ExpireVip = DateTime.FromBinary(reader.ReadInt64("Character", "VipTime", 0));
            if (DateTime.Now > client.Player.ExpireVip)
            {
                if (client.Player.VipLevel > 1)
                    client.Player.VipLevel = 0;
            }
            client.Achievement = new AchievementCollection();
            client.Achievement.Load(reader.ReadString("Character", "Achivement", ""));
            client.Player.GiantGhasomKill = reader.ReadUInt32("Character", "GiantGhasomKill", 0);
            client.Player.GiantGhasomKillClaimed = reader.ReadBool("Character", "GiantGhasomKillClaimed", false);
            client.Player.WHMoney = reader.ReadInt64("Character", "WHMoney", 0);
            client.Player.BlessTime = reader.ReadUInt32("Character", "BlessTime", 0);
            client.Player.SpouseUID = reader.ReadUInt32("Character", "SpouseUID", 0);
            client.Player.HeavenBlessing = reader.ReadInt32("Character", "HeavenBlessing", 0);
            client.Player.HeavenBlessTime = new Extensions.Time32(reader.ReadUInt32("Character", "LostTimeBlessing", 0));
            client.Player.HuntingBlessing = reader.ReadUInt32("Character", "HuntingBlessing", 0);
            client.Player.OnlineTrainingPoints = reader.ReadUInt32("Character", "OnlineTrainingPoints", 0);
            client.Player.JoinOnflineTG = DateTime.FromBinary(reader.ReadInt64("Character", "JoinOnflineTG", 0));
            client.Player.RateExp = reader.ReadUInt32("Character", "RateExp", 0);
            client.Player.DExpTime = reader.ReadUInt32("Character", "DExpTime", 0);
            client.Player.DailySpiritBeadCount = reader.ReadUInt32("Character", "DailySpiritBeadCount", 0);
            /*if (client.Player.RateExp > 2)
            {
                client.Player.RateExp = 1;
            }*/
            #region CQUESTS
            client.Player.helpladytime = reader.ReadByte("Character", "HelpLady", 0);
            client.Player.BattleFieldPoints = reader.ReadUInt32("Character", "Battlefieldpoints", 0);
            client.Player.VotePoints = reader.ReadUInt32("Character", "VotePoints", 0);
            #endregion
            client.Player.Day = reader.ReadInt32("Character", "Day", 0);
            client.Player.BDExp = reader.ReadByte("Character", "BDExp", 0);
            try
            {
                client.Player.MiningAttempts = reader.ReadUInt32("Character", "MiningAttempts", 0);
                client.Player.ExpFruit = reader.ReadByte("Character", "ExpFruitUsed", reader.ReadByte("Character", "ExpFruit", 0));
                client.Player.SwordMaster = reader.ReadByte("Character", "SwordMaster", 0);
                client.Player.TeratoDragon = reader.ReadByte("Character", "TeratoDragon", 0);
                client.Player.PurbleBanshee = reader.ReadByte("Character", "PurbleBanshee", 0);
                client.Player.ExchangeNormalAvaliability = reader.ReadUInt32("Character", "ExchangeNormalAvaliability", 0);
                client.Player.ExchangehighAvaliability = reader.ReadUInt32("Character", "ExchangehighAvaliability", 0);
                client.Player.CPBoundPack = reader.ReadByte("Character", "CPBoundPack", 0);
                client.Player.ClaimedBCPToday = reader.ReadBool("Character", "CPBoundPackToday", false);
                client.Player.FateFruitUsed = reader.ReadByte("Character", "FateFruitUsed", 0);
                client.Player.AnswerdToday = reader.ReadUInt32("Character", "AnswerdToday", 0);
                client.Player.ConquerPointsLimitaion = reader.ReadUInt32("Character", "ConquerPointsLimitaion", 0);
                client.Player.ConquerPointDropLimitLayer = reader.ReadByte("Character", "ConquerPointDropLimitLayer", 0);
                if (client.Player.ConquerPointDropLimitLayer > 1)
                {
                    client.Player.ConquerPointsDropStamp = DateTime.FromBinary(reader.ReadInt64("Character", "ConquerPointsDropStamp", 0));
                }
            }
            catch (Exception ex)
            {
                MyConsole.SaveException(ex);
            }
            client.Player.ExpBallUsed = reader.ReadByte("Character", "ExpBallUsed", 0);

            client.Player.RechargeProgress = (Role.Player.RechargeType)reader.ReadUInt32("Character", "RechargeProgress", 0);


            DataCore.LoadClient(client.Player);

            client.Player.GuildID = reader.ReadUInt32("Character", "GuildID", 0);
            client.Player.GuildRank = (Role.Flags.GuildMemberRank)reader.ReadUInt32("Character", "GuildRank", 200);
            if (client.Player.GuildID != 0)
            {
                Role.Instance.Guild myguild;
                if (Role.Instance.Guild.GuildPoll.TryGetValue(client.Player.GuildID, out myguild))
                {
                    client.Player.MyGuild = myguild;
                    Role.Instance.Guild.Member member;
                    if (myguild.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.IsOnline = true;
                        client.Player.GuildID = (ushort)myguild.Info.GuildID;
                        client.Player.MyGuildMember = member;
                        client.Player.GuildRank = member.Rank;
                        client.Player.GuildBattlePower = myguild.ShareMemberPotency(member.Rank);


                    }
                    else
                    {
                        client.Player.MyGuild = null;
                        client.Player.GuildID = 0;
                        client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                    }
                }
                else
                {
                    client.Player.MyGuild = null;
                    client.Player.GuildID = 0;
                    client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                }
            }

            // union: as a member of a guild of the union or as an individual member (Union.Members)
            try
            {
                Role.Instance.Union.BindPlayer(client);
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }



            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.SubClass.Load(reader.ReadString("Character", "SubProfInfo", ""));
            client.Player.SubClass.CreateSpawn(client);

            if (Role.Instance.Chi.ChiPool.ContainsKey(UID))
            {
                client.Player.MyChi = Role.Instance.Chi.ChiPool[UID];
                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
            }
            else
                client.Player.MyChi = new Role.Instance.Chi(UID);

            if (Role.Instance.Flowers.ClientPoll.ContainsKey(UID))
                client.Player.Flowers = Role.Instance.Flowers.ClientPoll[UID];
            else
                client.Player.Flowers = new Role.Instance.Flowers(UID, client.Player.Name);
            string flowerStr = reader.ReadString("Character", "Flowers", "");
            Database.DBActions.ReadLine Linereader = new DBActions.ReadLine(flowerStr, '/');
            client.Player.Flowers.FreeFlowers = Linereader.Read((uint)0);



            Role.Instance.Nobility nobility;
            if (Program.NobilityRanking.TryGetValue(UID, out nobility))
            {
                client.Player.Nobility = nobility;
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
            else
            {
                client.Player.Nobility = new Role.Instance.Nobility(client);
                client.Player.Nobility.Donation = reader.ReadUInt64("Character", "DonationNobility", 0);
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }


            Role.Instance.JiangHu Jiang;
            if (Role.Instance.JiangHu.Poll.TryGetValue(client.Player.UID, out Jiang))
            {
                client.Player.MyJiangHu = Jiang;
                client.Player.MyJiangHu.Level = (byte)client.Player.Level;
                client.Player.MyJiangHu.CountDownMode = DateTime.Now;
            }


            Role.Instance.Associate.MyAsociats Associate;
            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
            {
                client.Player.Associate = Associate;
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
                if (client.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Mentor))
                {
                    foreach (var member in client.Player.Associate.Associat[Role.Instance.Associate.Mentor].Values)
                    {
                        if (member.UID != 0)
                        {
                            Role.Instance.Associate.MyAsociats mentor;
                            if (Role.Instance.Associate.Associates.TryGetValue(member.UID, out mentor))
                            {
                                client.Player.MyMentor = mentor;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
            }
            client.Player.ClanUID = reader.ReadUInt32("Character", "ClanID", 0);
            if (client.Player.ClanUID != 0)
            {
                Role.Instance.Clan myclan;
                if (Role.Instance.Clan.Clans.TryGetValue(client.Player.ClanUID, out myclan))
                {
                    client.Player.MyClan = myclan;
                    Role.Instance.Clan.Member member;
                    if (myclan.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.Online = true;
                        client.Player.ClanName = myclan.Name;
                        client.Player.MyClanMember = member;
                        client.Player.ClanRank = (ushort)member.Rank;
                    }
                    else
                    {
                        client.Player.MyClan = null;
                        client.Player.ClanUID = 0;
                        client.Player.ClanRank = 0;
                    }
                }
                else
                    client.Player.ClanUID = 0;
            }
            client.Player.SpecialWingID = reader.ReadUInt32("Character", "SpecialWingID", 0);
            client.Player.FirstRebornLevel = reader.ReadByte("Character", "FRL", 0);
            client.Player.SecoundeRebornLevel = reader.ReadByte("Character", "SRL", 0);
            client.Player.Reincarnation = reader.ReadBool("Character", "Reincanation", false);
            client.Player.LotteryEntries = reader.ReadByte("Character", "LotteryEntries", 0);
            Database.LotteryTable.LoadPrize(client.Player, reader.ReadString("Character", "LotteryPrize", "0"));
            client.Player.DbTry = reader.ReadBool("Character", "DbTry", false);
            client.Player.MSConquer = reader.ReadBool("Character", "MSConquer", false);
            client.Player.MRConquer = reader.ReadBool("Character", "MRConquer", false);
            client.DemonExterminator.ReadLine(reader.ReadString("Character", "DemonEx", "0/0/"));

            client.Player.MyKillerUID = reader.ReadUInt32("Character", "PkUID", 0);
            client.Player.MyKillerName = reader.ReadString("Character", "PkName", "None");
            client.Player.CursedTimer = reader.ReadInt32("Character", "Cursed", 0);
            client.HeroRewards.Load(reader.ReadBigString("Character", "HeroRewards", ""));
            client.Activeness.Load(reader.ReadBigString("Character", "Activeness", ""));
            client.Player.AtiveQuestApe = reader.ReadUInt32("Character", "enervant", 0);

            client.Player.AparenceType = reader.ReadUInt32("Character", "AparenceType", 0);

            client.Player.HitShoot = reader.ReadUInt32("Character", "HitShoot", 0);
            client.Player.MisShoot = reader.ReadUInt32("Character", "MisShoot", 0);
            client.Player.ArenaKills = reader.ReadUInt32("Character", "ArenaKills", 0);
            client.Player.ArenaDeads = reader.ReadUInt32("Character", "ArenaDeads", 0);

            client.Player.HitSBShoot = reader.ReadUInt32("Character", "HitSBShoot", 0);
            client.Player.MisSBShoot = reader.ReadUInt32("Character", "MisSBShoot", 0);
            client.Player.ArenaSBKills = reader.ReadUInt32("Character", "ArenaSBKills", 0);
            client.Player.ArenaSBDeads = reader.ReadUInt32("Character", "ArenaSBDeads", 0);

            client.Player.TournamentKills = reader.ReadUInt32("Character", "TKills", 0);
            client.Player.OnlineMinutes = reader.ReadUInt32("Character", "OnlineMinutes", 0);
            client.Player.EpicQuestChance = reader.ReadUInt32("Character", "EpicQuestChance", 0);
            client.Player.HistoryChampionPoints = reader.ReadUInt32("Character", "HistoryChampionPoints", 0);

            client.Player.AddChampionPoints(reader.ReadUInt32("Character", "ChampionPoints", 0), false);
            client.Player.TodayChampionPoints = reader.ReadUInt32("Character", "TodayChampionPoints", 0);
            client.Player.DailySpiritBeadItem = reader.ReadUInt32("Character", "DailySpiritBeadItem", 0);

            LoadSpecialTitles(client, reader.ReadString("Character", "SpecialTitles", "0,0,"));
            LoadSecurityPassword(reader.ReadString("Character", "SecurityPass", "0,0,0"), client);
            client.Player.TCCaptainTimes = reader.ReadByte("Character", "TCT", 0);
            client.Player.RacePoints = reader.ReadUInt32("Character", "RacePoints", 0);
            client.Player.SteedRaceBestTime = reader.ReadUInt32("Character", "SteedRaceBest", 0);
            client.Player.NameEditCount = reader.ReadUInt16("Character", "NameEditCount", 0);
            client.Player.MainFlag = (Role.Player.MainFlagType)reader.ReadUInt32("Character", "ClaimStateGift", 0);
            client.Player.CountryID = reader.ReadUInt16("Character", "CountryID", 0);

            client.Player.InventorySashCount = reader.ReadUInt16("Character", "InventorySashCount", 0);
            client.Player.MyFootBallPoints = reader.ReadUInt32("Character", "MyFootBallPoints", 0);
            client.Player.ExpProtection = reader.ReadUInt32("Character", "ExpProtection", 0);
            client.BanCount = reader.ReadByte("Character", "BanCount", 0);
            client.Player.KingDomExploits = reader.ReadUInt32("Character", "KingDomExploits", 0);


            client.Player.BuyKingdomDeeds = reader.ReadByte("Character", "BuyKingdomDeeds", 0);
            client.Player.KingDomDeeds = reader.ReadUInt32("Character", "KingDomDeeds", 0);
            // client.Player.KingDomGold = reader.ReadUInt32("Character", "KingDomGold", 0);

            client.MyExchangeShop.Load(reader.ReadString("Character", "ExchangeShop", "0"));
            client.Player.ExtraAtributes = reader.ReadUInt16("Character", "ExtraAtributes", 0);

            client.Player.OpenHousePack = reader.ReadByte("Character", "OpenHousePack", 0);
            client.Player.MyTowerOfMysteryLayer = reader.ReadByte("Character", "MyTowerOfMysteryLayer", 0);

            client.Player.ClaimTowerAmulets = reader.ReadByte("Character", "ClaimTowerAmulets", 0);
            client.Player.TOMClaimTeamReward = reader.ReadByte("Character", "TOMClaimTeamReward", 0);
            client.Player.MyTowerOfMysteryLayerElite = reader.ReadByte("Character", "MyTowerOfMysteryLayerElite", 0);
            client.Player.TowerOfMysterychallenge = reader.ReadByte("Character", "TowerOfMysterychallenge", 0);
            client.Player.TowerOfMysteryChallengeFlag = reader.ReadUInt32("Character", "TowerOfMysteryChallengeFlag", 0);
            client.Player.TOMSelectChallengeToday = reader.ReadByte("Character", "TOMSelectChallengeToday", 0);
            client.Player.TOMChallengeToday = reader.ReadByte("Character", "TOMChallengeToday", 0);
            client.Player.TOMRefreshReward = reader.ReadUInt32("Character", "TOMRefreshReward", 0);
            client.Player.JoinPowerArenaStamp = DateTime.FromBinary(reader.ReadInt64("Character", "JPAStamp", 0));
            client.Player.TOM_Reward = (Game.MsgTournaments.MsgTowerOfMystery.RewardTypes)reader.ReadByte("Character", "TOM_Reward", 0);

            client.Player.LoadEpicTrojan(reader.ReadString("Character", "EpicTrojan", "0/0/0/0/1/1/0/0/0/0/0/0/0/0/1/0/0/0/1"));

            client.Player.GiveFlowersToPerformer = reader.ReadInt32("Character", "GiveFlowersToPerformer", 0);

            client.Player.UseChiToken = reader.ReadByte("Character", "UseChiToken", 0);
            client.Player.CanChangeWindWalkerFree = DateTime.FromBinary(reader.ReadInt64("Character", "CanChangeWindWalkerFree", DateTime.Now.Ticks));
          
            try
            {
                LoadClientItems(client, binaryFiles.Items);
                LoadClientSpells(client, binaryFiles.Spells);
                LoadClientProfs(client, binaryFiles.Profs);
                RoleQuests.Load(client, binaryFiles.Quests);
                Halo_Flag.Load(client);//Halo_Flag
                Role.Instance.House.Load(client, binaryFiles.House);
                Database.MailService.Load(client);//mailbox (PlayersMail\<UID>.bin)
                Game.MsgServer.BlackList.Load(client.Player.UID);//black list (PlayersBlackList\<UID>.txt)
            }
            catch (InvalidDataException e)
            {
                Security.Guard.Audit("load-error", UID, client.Player.Name, e.Message + ", login refused");
                MyConsole.WriteLine("Login refused for " + UID + ": " + e.Message);
                try
                {
                    if (client.Socket != null)
                        client.Socket.Disconnect();
                }
                catch { }
                throw;
            }

            ResetingEveryDay(client);


            Role.Instance.Confiscator Container;
            if (Server.QueueContainer.PollContainers.TryGetValue(client.Player.UID, out Container))
                client.Confiscator = Container;
            try
            {
                client.Player.Associate.OnLoading(client);
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

            if (!Role.Instance.InnerPower.InnerPowerPolle.TryGetValue(client.Player.UID, out client.Player.InnerPower))
                client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

            client.Player.InnerPower.UpdateStatus();

            if (Game.MsgTournaments.MsgArena.ArenaPoll.TryGetValue(client.Player.UID, out client.ArenaStatistic))
            {
                client.ArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                client.ArenaStatistic.ApplayInfo(client.Player);
                client.ArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
            }

            if (Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryGetValue(client.Player.UID, out client.TeamArenaStatistic))
            {
                client.TeamArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                client.TeamArenaStatistic.ApplayInfo(client.Player);
                client.TeamArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            }
           
            client.FullLoading = true;
        }
        public static ushort CalculateEnlighten(Role.Player player)
        {
            if (player.Level < 90)
                return 0;
            ushort val = 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Knight || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Baron)
                val += 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Earl || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Duke)
                val += 200;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Prince)
                val += 300;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.King)
                val += 400;
            if (player.VipLevel <= 3)
                val += 100;
            if (player.VipLevel > 3 && player.VipLevel <= 5)
                val += 200;
            if (player.VipLevel > 5)
                val += 300;

            return val;
        }
        public static uint CalculateDropLimitation(Role.Player player)
        {
         //دبل دروب
            uint Value = 100;
      
            Value *= player.ConquerPointDropLimitLayer;
            return Math.Max(0, Value);
        }
       
        public static string GetSecurityPassword(Client.GameClient user)
        {
            Database.DBActions.WriteLine writer = new DBActions.WriteLine(',');
            writer.Add(user.Player.SecurityPassword);
            writer.Add(user.Player.OnReset);
            writer.Add(user.Player.ResetSecurityPassowrd.Ticks);
            return writer.Close();
        }
        public static void LoadSecurityPassword(string line,Client.GameClient user)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, ',');
            user.Player.SecurityPassword = reader.Read((uint)0);
            user.Player.OnReset = reader.Read((uint)0);
            if (user.Player.OnReset == 1)
            {
                user.Player.ResetSecurityPassowrd = DateTime.FromBinary(reader.Read((long)0));
                if (DateTime.Now > user.Player.ResetSecurityPassowrd)
                {
                    user.Player.OnReset = 0;
                    user.Player.SecurityPassword = 0;
                }
            }
           
        }
        public static string GetSpecialTitles(Client.GameClient user)
        {
            Database.DBActions.WriteLine writer = new DBActions.WriteLine(',');
            writer.Add((uint)user.Player.SpecialTitles.Count);
            foreach (var title in user.Player.SpecialTitles)
            {
                writer.Add((uint)title);
                if (user.Player.SpecialTitleID / 10000 == (uint)title || user.Player.SpecialWingID / 10000 == (uint)title)
                    writer.Add((byte)1);
                else
                    writer.Add((byte)0);
            }
            return writer.Close();
        }
        public static void LoadSpecialTitles(Client.GameClient user, string line)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, ',');
            uint count = reader.Read((uint)0);
            for (int x = 0; x < count; x++)
            {
                uint Title = reader.Read((uint)0);
                uint Active = reader.Read((uint)0);
                user.Player.SpecialTitles.Add((Game.MsgServer.MsgTitleStorage.TitleType)Title);
                if (Active == 1)
                {
                    Database.TitleStorage dbtitle;
                    if (Database.TitleStorage.Titles.TryGetValue(Title, out dbtitle))
                    {
                        if (dbtitle.ID <= 4001)
                        {
                            user.Player.SpecialTitleScore = dbtitle.Score;
                            user.Player.SpecialTitleID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                        }
                        else
                            user.Player.SpecialWingID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                    }
                }
            }
        }

      
       
    
        #region player binary files (validated load, atomic save)

        /// <summary>Paths chosen (and validated) for a character's binary files before anything is loaded.
        /// A null path means the file does not exist yet (new character).</summary>
        public class PlayerBinaryFiles
        {
            public string Items, Spells, Profs, Quests, House;
        }

        static string PlayerFile(string folder, uint UID)
        {
            return Program.ServerConfig.DbLocation + "\\" + folder + "\\" + UID + ".bin";
        }

        /// <summary>shell.ini [Security] LenientLegacyLoad (default 1, read by Server.Initialize). When a player file
        /// fails validation and has no valid .bak, 1 loads it with the tolerant readers (as before the hardening) after
        /// copying it to "&lt;file&gt;.corrupt-&lt;time&gt;"; 0 refuses the login.</summary>
        public static bool LenientLegacyLoad = true;

        /// <summary>
        /// Returns the file to load: the file itself if valid, otherwise "&lt;file&gt;.bak" if valid (the bad file is
        /// moved aside so the next save does not rotate it over the good backup), or null if neither exists.
        /// When neither copy is valid: with LenientLegacyLoad the existing file is returned anyway (tolerant read,
        /// copy kept as .corrupt-*), otherwise InvalidDataException is thrown.
        /// </summary>
        public static string PickLoadPath(string path, Func<string, bool> validate, uint UID)
        {
            bool exists = File.Exists(path);
            if (exists)
            {
                int state = SafeValidate(validate, path);
                for (int retry = 0; state == ValidateIOError && retry < 5; retry++)
                {
                    System.Threading.Thread.Sleep(50);
                    state = SafeValidate(validate, path);
                }
                if (state == ValidateOK)
                    return path;
                if (state == ValidateIOError)
                {
                    // can't read it right now: refuse rather than fall back to older data
                    Security.Guard.Audit("load-error", UID, "", "could not read " + path + ", login refused");
                    throw new InvalidDataException("Unreadable player file " + path);
                }
            }
            string bak = path + ".bak";
            bool bakExists = File.Exists(bak);
            if (!exists && !bakExists)
                return null;
            if (bakExists && SafeValidate(validate, bak) == ValidateOK)
            {
                if (exists)
                {
                    try
                    {
                        File.Move(path, path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                    catch (Exception e)
                    {
                        Security.Guard.Audit("load-error", UID, "", "could not move aside " + path + ": " + e.Message);
                        throw new InvalidDataException("Corrupt player file " + path);
                    }
                }
                Security.Guard.Audit("load-fallback", UID, "", (exists ? "invalid " : "missing ") + path + ", loaded .bak");
                return bak;
            }
            if (exists && LenientLegacyLoad)
            {
                // Saves written before the hardening may be slightly inconsistent and have no .bak yet:
                // read what is there (pre-hardening behaviour) instead of locking the player out.
                // The next save writes a clean file; the original is kept for inspection.
                string reason = "invalid " + path + (bakExists ? " and .bak" : ", no .bak");
                string copy = path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                try
                {
                    File.Copy(path, copy, true);
                    reason += ", original copied to " + Path.GetFileName(copy);
                }
                catch (Exception e)
                {
                    reason += ", copy failed: " + e.Message;
                }
                Security.Guard.Audit("legacy-load", UID, "", reason + ", loaded with the tolerant reader (LenientLegacyLoad=1)");
                MyConsole.WriteLine("Legacy load for " + UID + ": " + path + " failed validation, loaded as-is (see Security_Logs).");
                return path;
            }
            Security.Guard.Audit("load-error", UID, "", "invalid " + path + (bakExists ? " and .bak" : "") + ", login refused");
            throw new InvalidDataException("Corrupt player file " + path);
        }

        /// <summary>Reads an integer from shell.ini (next to the executable), independent of DbLocation.</summary>
        public static int ReadShellInt(string section, string key, int def)
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile();
            ini.FileName = Directory.GetCurrentDirectory() + "\\shell.ini";
            return ini.ReadInt32(section, key, def);
        }
        const int ValidateOK = 0, ValidateBad = 1, ValidateIOError = 2;
        static int SafeValidate(Func<string, bool> validate, string path)
        {
            try
            {
                return validate(path) ? ValidateOK : ValidateBad;
            }
            catch (IOException e)
            {
                if (e is EndOfStreamException)
                    return ValidateBad;
                return ValidateIOError;
            }
            catch (UnauthorizedAccessException)
            {
                return ValidateIOError;
            }
            catch
            {
                return ValidateBad;
            }
        }

        /// <summary>File layout: int count, count records of recordSize bytes.</summary>
        public static bool ValidateCountedFile(string path, int recordSize)
        {
            long len = new FileInfo(path).Length;
            if (len < sizeof(int))
                return false;
            int count;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
                count = br.ReadInt32();
            return count >= 0 && sizeof(int) + (long)count * recordSize == len;
        }

        /// <summary>Walks an items file: int count, count DBItem (each followed by DepositeCount DBItem),
        /// then int count, count Perfection. Reports the highest item UID seen.</summary>
        public unsafe static bool ValidateItemsFile(string path, out uint maxUID)
        {
            uint ignored, ignoredMax;
            return ValidateItemsFile(path, out maxUID, uint.MaxValue, out ignored, out ignoredMax);
        }

        /// <summary>Same walk; UIDs above uidLimit are not counted in maxUID but reported (count + highest).</summary>
        public unsafe static bool ValidateItemsFile(string path, out uint maxUID, uint uidLimit, out uint implausible, out uint implausibleMax)
        {
            uint outOfRange;
            return ValidateItemsFile(path, out maxUID, 0, uint.MaxValue, uidLimit, out implausible, out implausibleMax, out outOfRange);
        }

        /// <summary>Same walk; UIDs outside [rangeMin, rangeMax] are only counted in outOfRange, UIDs inside the range
        /// but above uidLimit are reported as implausible (count + highest).</summary>
        public unsafe static bool ValidateItemsFile(string path, out uint maxUID, uint rangeMin, uint rangeMax, uint uidLimit, out uint implausible, out uint implausibleMax, out uint outOfRange)
        {
            outOfRange = 0;
            maxUID = 0;
            implausible = 0;
            implausibleMax = 0;
            byte[] data = File.ReadAllBytes(path);
            long len = data.Length;
            int itemSize = sizeof(ClientItems.DBItem);
            int perfSize = sizeof(ClientItems.Perfection);
            if (len < sizeof(int))
                return false;
            fixed (byte* p = data)
            {
                long pos = 0;
                int count = *(int*)p;
                pos += sizeof(int);
                if (count < 0 || count > (len - pos) / itemSize)
                    return false;
                for (int x = 0; x < count; x++)
                {
                    if (pos + itemSize > len)
                        return false;
                    ClientItems.DBItem* item = (ClientItems.DBItem*)(p + pos);
                    pos += itemSize;
                    if (item->UID != 0 && (item->UID < rangeMin || item->UID > rangeMax))
                        outOfRange++;
                    else if (item->UID > uidLimit)
                    {
                        implausible++;
                        if (item->UID > implausibleMax)
                            implausibleMax = item->UID;
                    }
                    else if (item->UID > maxUID)
                        maxUID = item->UID;
                    uint deposits = item->DepositeCount;
                    if (deposits > (len - pos) / itemSize)
                        return false;
                    for (uint i = 0; i < deposits; i++)
                    {
                        ClientItems.DBItem* dep = (ClientItems.DBItem*)(p + pos);
                        pos += itemSize;
                        if (dep->UID != 0 && (dep->UID < rangeMin || dep->UID > rangeMax))
                            outOfRange++;
                        else if (dep->UID > uidLimit)
                        {
                            implausible++;
                            if (dep->UID > implausibleMax)
                                implausibleMax = dep->UID;
                        }
                        else if (dep->UID > maxUID)
                            maxUID = dep->UID;
                    }
                }
                if (pos == len)
                    return true;//no perfection section
                if (pos + sizeof(int) > len)
                    return false;
                int perfCount = *(int*)(p + pos);
                pos += sizeof(int);
                return perfCount >= 0 && pos + (long)perfCount * perfSize == len;
            }
        }
        public static bool ValidateItemsFile(string path)
        {
            uint max;
            return ValidateItemsFile(path, out max);
        }

        /// <summary>Resolves all binary files of a character. Throws InvalidDataException if any is unusable.</summary>
        public unsafe static PlayerBinaryFiles ResolvePlayerFiles(uint UID)
        {
            PlayerBinaryFiles files = new PlayerBinaryFiles();
            int spellSize = sizeof(ClientSpells.DBSpell);
            int profSize = sizeof(ClientProficiency.DBProf);
            files.Items = PickLoadPath(PlayerFile("PlayersItems", UID), ValidateItemsFile, UID);
            files.Spells = PickLoadPath(PlayerFile("PlayersSpells", UID), p => ValidateCountedFile(p, spellSize), UID);
            files.Profs = PickLoadPath(PlayerFile("PlayersProfs", UID), p => ValidateCountedFile(p, profSize), UID);
            files.Quests = PickLoadPath(PlayerFile("Quests", UID), RoleQuests.ValidateFile, UID);
            files.House = PickLoadPath(PlayerFile("Houses", UID), Role.Instance.House.ValidateFile, UID);
            return files;
        }

        /// <summary>Highest UID accepted by the high-water scan, whatever the saved counter.</summary>
        public const uint MaxPlausibleItemUID = 2000000000;
        /// <summary>How far above the saved ItemUID counter a stored UID may be before it is considered bogus.</summary>
        public const uint MaxItemUIDAboveCounter = 50000000;

        /// <summary>Highest item UID stored in PlayersItems\*.bin (and their .bak copies).
        /// Implausible UIDs (above MaxPlausibleItemUID, or more than MaxItemUIDAboveCounter above the saved
        /// counter when one is known) are ignored and audited instead of pushing the counter towards uint.MaxValue.</summary>
        public static uint ScanHighestItemUID(uint savedCounter)
        {
            return ScanHighestItemUID(savedCounter, 1, MaxPlausibleItemUID);
        }

        /// <summary>Same scan for a server that owns the item UID range [rangeMin, rangeMax] (ClusterConfig).
        /// UIDs outside the range are legal (items that came from another server) and are skipped without an audit;
        /// only one summary line is printed.</summary>
        public static uint ScanHighestItemUID(uint savedCounter, uint rangeMin, uint rangeMax)
        {
            uint highest = 0;
            uint limit = Math.Min(MaxPlausibleItemUID, rangeMax);
            if (savedCounter > 0 && (ulong)savedCounter + MaxItemUIDAboveCounter < limit)
                limit = savedCounter + MaxItemUIDAboveCounter;
            long foreign = 0;
            uint foreignFiles = 0;
            try
            {
                string folder = Program.ServerConfig.DbLocation + "\\PlayersItems\\";
                if (!Directory.Exists(folder))
                    return 0;
                foreach (string fname in Directory.GetFiles(folder))
                {
                    if (!fname.EndsWith(".bin", StringComparison.OrdinalIgnoreCase) && !fname.EndsWith(".bin.bak", StringComparison.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        uint max, implausible, implausibleMax, outOfRange;
                        if (ValidateItemsFile(fname, out max, rangeMin, rangeMax, limit, out implausible, out implausibleMax, out outOfRange))
                        {
                            if (outOfRange > 0)
                            {
                                foreign += outOfRange;
                                foreignFiles++;
                            }
                            if (max > highest)
                                highest = max;
                            if (implausible > 0)
                            {
                                Security.Guard.Audit("item-uid-implausible", 0, "", fname + ": " + implausible + " item(s) with UID above " + limit +
                                    " ignored by the UID scan (highest " + implausibleMax + ", saved counter " + savedCounter + ")");
                                MyConsole.WriteLine("Item UID scan: " + implausible + " implausible item UID(s) in " + fname + " ignored (highest " + implausibleMax + "), see Security_Logs.");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Security.Guard.Audit("load-error", 0, "", "item uid scan " + fname + ": " + e.Message);
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
            if (foreign > 0)
                MyConsole.WriteLine("Item UID scan: " + foreign + " item(s) in " + foreignFiles + " file(s) have UIDs outside this server's range " + rangeMin + "-" + rangeMax + " (from other servers), not counted.");
            return highest;
        }

        public unsafe static void LoadClientItems(Client.GameClient client)
        {
            LoadClientItems(client, PickLoadPath(PlayerFile("PlayersItems", client.Player.UID), ValidateItemsFile, client.Player.UID));
        }
        public unsafe static void LoadClientItems(Client.GameClient client, string path)
        {
            if (path == null)
                return;
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open))
                throw new InvalidDataException("Cannot open " + path);//never continue with an empty character
            {
                ClientItems.DBItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                Dictionary<uint, MsgGameItem> InContainer = new Dictionary<uint, MsgGameItem>();
                for (int x = 0; x < ItemCount; x++)
                {
                    binary.Read(&Item, sizeof(ClientItems.DBItem));
                    if (Item.ITEM_ID == 750000)//demonExterminator jar
                        client.DemonExterminator.ItemUID = Item.UID;
                    Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                    if (Item.DepositeCount != 0)
                    {
                        uint DepositeCount = Item.DepositeCount;
                        for (int i = 0; i < DepositeCount; i++)
                        {
                            binary.Read(&Item, sizeof(ClientItems.DBItem));
                            if (Item.ITEM_ID == 750000)
                                client.DemonExterminator.ItemUID = Item.ITEM_ID;

                            Game.MsgServer.MsgGameItem DepositeItem = Item.GetDataItem();
                            if (client.Player.GuildID == 0)
                                DepositeItem.Inscribed = 0;
                            ClienItem.Deposite.TryAdd(DepositeItem.UID, DepositeItem);
                            if (!InContainer.ContainsKey(DepositeItem.UID))
                                InContainer.Add(DepositeItem.UID, DepositeItem);
                        }
                    }
                    if (Item.WH_ID != 0)
                    {
                        if (Item.WH_ID == 100)
                        {
                            client.MyWardrobe.AddItem(ClienItem);
                            if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                            }
                        }
                        else
                        {
                            if (!client.Warehouse.ClientItems.ContainsKey(Item.WH_ID))
                                client.Warehouse.ClientItems.TryAdd(Item.WH_ID, new System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Warehouse.ClientItems[Item.WH_ID].TryAdd(Item.UID, ClienItem);
                        }
                    }
                    else
                    {
                        if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                        }
                        else if (Item.Position == 0)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Inventory.AddDBItem(ClienItem);
                        }
                    }
                }
                if (!binary.Read(&ItemCount, sizeof(int)))
                    ItemCount = 0;//file written before the perfection section existed
                for (int x = 0; x < ItemCount; x++)
                {
                    ClientItems.Perfection info = new ClientItems.Perfection();
                    binary.Read(&info, sizeof(ClientItems.Perfection));
                    if (InContainer.ContainsKey(info.ItemUID))
                    {
                        var item = InContainer[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    else if (client.Equipment.ClientItems.ContainsKey(info.ItemUID))
                    {
                        var item = client.Equipment.ClientItems[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    else if (client.Inventory.ClientItems.ContainsKey(info.ItemUID))
                    {
                        var item = client.Inventory.ClientItems[info.ItemUID];
                        item.PerfectionLevel = info.Level;
                        item.OwnerUID = info.OwnerUID;
                        item.OwnerName = info.OwnerName;
                        item.PerfectionProgress = info.Progres;
                        item.Signature = info.SpecialText;
                        continue;
                    }
                    bool found = false;
                    foreach (var WH in client.Warehouse.ClientItems.Values)
                    {
                        if (found == false)
                        {
                            if (WH.ContainsKey(info.ItemUID))
                            {
                                found = true;
                                var item = WH[info.ItemUID];
                                item.PerfectionLevel = info.Level;
                                item.OwnerUID = info.OwnerUID;
                                item.OwnerName = info.OwnerName;
                                item.PerfectionProgress = info.Progres;
                                item.Signature = info.SpecialText;
                            }
                        }
                    }
                }
                binary.Close();
            }
        }

        public unsafe static void SaveClientInbox(Client.GameClient client)
        {
            var prizes = client.MailBox.Values.Where(p => p.ID != 0).ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersMail\\" + client.Player.UID + ".bin", FileMode.Create))
                {
                    ClientMail.DBPrize DBprize = new ClientMail.DBPrize();
                    int CountPrizes = prizes.Length;
                    binary.Write(&CountPrizes, sizeof(int));
                    foreach (var prize in prizes)
                    {
                        DBprize.GetDBPrize(prize);
                        binary.Write(&DBprize, sizeof(ClientMail.DBPrize));
                    }
                    binary.Close();
                }
            }
        }
        public unsafe static void LoadClientInbox(Client.GameClient client)
        {
            string path;
            try
            {
                int size = sizeof(ClientMail.DBPrize);
                path = PickLoadPath(PlayerFile("PlayersMail", client.Player.UID), p => ValidateCountedFile(p, size), client.Player.UID);
            }
            catch (InvalidDataException)
            {
                return;
            }
            if (path == null)
                return;
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open))
                throw new InvalidDataException("Cannot open " + path);//never continue with an empty character
            {
                ClientMail.DBPrize DBprize;
                int CountPrizes;
                binary.Read(&CountPrizes, sizeof(int));
                for (int x = 0; x < CountPrizes; x++)
                {
                    binary.Read(&DBprize, sizeof(ClientMail.DBPrize));
                    var ClientPrize = DBprize.GetClientPrize();
                    client.MailBox.TryAdd(ClientPrize.ID, ClientPrize);
                }
                binary.Close();
            }
        }

        public unsafe static void SaveClientItems(Client.GameClient client)
        {
            // Count and records come from the same snapshot so the header always matches the data.
            var items = client.AllMyItems().ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Create))
                {
                    Dictionary<uint, MsgGameItem> InContainer = new Dictionary<uint, MsgGameItem>();
                    ClientItems.DBItem DBItem = new ClientItems.DBItem();
                    int ItemCount = items.Length;
                    binary.Write(&ItemCount, sizeof(int));
                    foreach (var item in items)
                    {

                        var poss = Database.ItemType.ItemPosition(item.ITEM_ID);
                        if (poss == (ushort)Role.Flags.ConquerItem.Wing)
                        {
                            if (item.Bless == 1)
                            {
                                item.Bless = 0;
                            }
                        }
                        var deposits = item.Deposite.Values.ToArray();
                        DBItem.GetDBItem(item);
                        DBItem.DepositeCount = (uint)deposits.Length;
                        binary.Write(&DBItem, sizeof(ClientItems.DBItem));
                        foreach (var DepositItem in deposits)
                        {
                            DBItem.GetDBItem(DepositItem);
                            binary.Write(&DBItem, sizeof(ClientItems.DBItem));
                            if ((DepositItem.PerfectionLevel > 0 || DepositItem.PerfectionProgress > 0) && DepositItem.IsEquip)
                                if (!InContainer.ContainsKey(DepositItem.UID))
                                    InContainer.Add(DepositItem.UID, DepositItem);
                        }
                    }

                    var perfection = client.AllPerfectionItems().ToArray();
                    ItemCount = perfection.Length + InContainer.Count;
                    binary.Write(&ItemCount, sizeof(int));
                    foreach (var item in perfection)
                    {
                        var info = DBItem.GetPerfectionInfo(item);
                        binary.Write(&info, sizeof(ClientItems.Perfection));
                    }
                    foreach (var item in InContainer.Values)
                    {
                        var info = DBItem.GetPerfectionInfo(item);
                        binary.Write(&info, sizeof(ClientItems.Perfection));
                    }
                    if (!binary.Close())
                        MyConsole.WriteLine("Items save failed for " + client.Player.UID + ", previous file kept.");
                }
            }
        }
        public unsafe static void LoadClientProfs(Client.GameClient client)
        {
            int size = sizeof(ClientProficiency.DBProf);
            LoadClientProfs(client, PickLoadPath(PlayerFile("PlayersProfs", client.Player.UID), p => ValidateCountedFile(p, size), client.Player.UID));
        }
        public unsafe static void LoadClientProfs(Client.GameClient client, string path)
        {
            if (path == null)
                return;
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open))
                throw new InvalidDataException("Cannot open " + path);//never continue with an empty character
            {
                ClientProficiency.DBProf DBProf;
                int CountProf;
                binary.Read(&CountProf, sizeof(int));
                for (int x = 0; x < CountProf; x++)
                {
                    binary.Read(&DBProf, sizeof(ClientProficiency.DBProf));
                    var ClientProf = DBProf.GetClientProf();
                    client.MyProfs.ClientProf.TryAdd(ClientProf.ID, ClientProf);
                }
                binary.Close();
            }
        }
        public unsafe static void SaveClientProfs(Client.GameClient client)
        {
            var profs = client.MyProfs.ClientProf.Values.ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Create))
                {
                    ClientProficiency.DBProf DBProf = new ClientProficiency.DBProf();
                    int CountProf = profs.Length;
                    binary.Write(&CountProf, sizeof(int));
                    foreach (var prof in profs)
                    {
                        DBProf.GetDBSpell(prof);
                        binary.Write(&DBProf, sizeof(ClientProficiency.DBProf));
                    }
                    binary.Close();
                }
            }
        }
        public unsafe static void LoadClientSpells(Client.GameClient client)
        {
            int size = sizeof(ClientSpells.DBSpell);
            LoadClientSpells(client, PickLoadPath(PlayerFile("PlayersSpells", client.Player.UID), p => ValidateCountedFile(p, size), client.Player.UID));
        }
        public unsafe static void LoadClientSpells(Client.GameClient client, string path)
        {
            if (path == null)
                return;
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (!binary.Open(path, FileMode.Open))
                throw new InvalidDataException("Cannot open " + path);//never continue with an empty character
            {
                ClientSpells.DBSpell DBSpell;
                int CountSpell;
                binary.Read(&CountSpell, sizeof(int));
                for (int x = 0; x < CountSpell; x++)
                {
                    binary.Read(&DBSpell, sizeof(ClientSpells.DBSpell));
                    var clientSpell = DBSpell.GetClientSpell();
                    client.MySpells.ClientSpells.TryAdd(clientSpell.ID, clientSpell);
                }
                binary.Close();
            }
        }
        public unsafe static void SaveClientSpells(Client.GameClient client)
        {
            var spells = client.MySpells.ClientSpells.Values.ToArray();
            using (WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile())
            {
                if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Create))
                {
                    ClientSpells.DBSpell DBSpell = new ClientSpells.DBSpell();
                    int SpellCount = spells.Length;
                    binary.Write(&SpellCount, sizeof(int));
                    foreach (var spell in spells)
                    {
                        DBSpell.GetDBSpell(spell);
                        binary.Write(&DBSpell, sizeof(ClientSpells.DBSpell));
                    }
                    binary.Close();
                }
            }
        }
        #endregion
        public static void CreateCharacte(Client.GameClient client)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");
            write.Write<uint>("Character", "UID", client.Player.UID);
            write.Write<ushort>("Character", "Body", client.Player.Body);
            write.Write<ushort>("Character", "Face", client.Player.Face);
            write.WriteString("Character", "Name", client.Player.Name);
            write.Write<byte>("Character", "Class", client.Player.Class);
            write.Write<uint>("Character", "Map", client.Player.Map);
            write.Write<ushort>("Character", "X", client.Player.X);
            write.Write<ushort>("Character", "Y", client.Player.Y);
            client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
            client.ArenaStatistic.ApplayInfo(client.Player);
            client.ArenaStatistic.Info.ArenaPoints = 4000;
            Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);

            client.Player.Nobility = new Role.Instance.Nobility(client);

            client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
            client.TeamArenaStatistic.ApplayInfo(client.Player);
            client.TeamArenaStatistic.Info.ArenaPoints = 4000;
         
            Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);

            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
            client.Player.Associate.MyClient = client;
            client.Player.Associate.Online = true;


            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);
            
            client.Achievement = new AchievementCollection();
            client.Player.JustCreated = true;
            client.FullLoading = true;

        }
       
        public static bool AllowCreate(uint UID)
        {
            return !File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + UID + ".ini");
            // WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Accounts\\" + Account + ".ini");
            // return reader.ReadUInt32("Account", "UID", 1000000) == 1000000;
        }
        public static void UpdateGuildMember(Role.Instance.Guild.Member Member)
        {
            lock (SaveLockOf(Member.UID))
            {
                // a still queued save of this character (e.g. its logout save) is written first, so it can't undo this
                FlushPendingSaves(Member.UID, null);
                InvalidatePlayerIni(Member.UID);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
                //  write.Write<uint>("Character", "GuildID", 0);
                write.Write<ushort>("Character", "GuildRank", 0);
            }
        }
        public static void UpdateGuildMember(Role.Instance.Guild.UpdateDB Member)
        {
            lock (SaveLockOf(Member.UID))
            {
                // a still queued save of this character (e.g. its logout save) is written first, so it can't undo this
                FlushPendingSaves(Member.UID, null);
                InvalidatePlayerIni(Member.UID);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
                write.Write<ushort>("Character", "GuildRank", 0);
                write.Write<ushort>("Character", "GuildID", 0);
            }
        }
        public static void UpdateUnionMember(Role.Instance.Union.Member Member)
        {
            lock (SaveLockOf(Member.UID))
            {
                // a still queued save of this character (e.g. its logout save) is written first, so it can't undo this
                FlushPendingSaves(Member.UID, null);
                InvalidatePlayerIni(Member.UID);
                UpdateUnionMemberCore(Member);
            }
        }
        static void UpdateUnionMemberCore(Role.Instance.Union.Member Member)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            if (Member.ReceiveKick == 0)
            {
                // UnionUID is the union's id (it used to be written with the member's own UID, which lost
                // individual members on the next restart)
                if (Member.UnionID != 0)
                    write.Write<uint>("Character", "UnionUID", Member.UnionID);
                write.Write<uint>("Character", "UnionRank", (uint)Member.Rank);
                write.Write<uint>("Character", "UnionExploits", Member.Exploits);
                write.Write<uint>("Character", "Treasury", Member.MyTreasury);
                write.Write<uint>("Character", "UnionAllowanceDay", Member.AllowanceDay);
                write.Write<uint>("Character", "UnionStipend", Member.StipendClaimed);
            }
            else
            {
                write.Write<uint>("Character", "UnionUID", 0);
                write.Write<uint>("Character", "UnionRank", 0);
                write.Write<uint>("Character", "UnionExploits", 0);
                write.Write<uint>("Character", "UnionGoldBrick", 0);
            }
        }
        public static void UpdateMapRace(Role.GameMap map)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\maps\\" + map.ID + ".ini");
            write.Write<uint>("info", "race_record", map.RecordSteedRace);
        }
        public static void UpdateClanMember(Role.Instance.Clan.Member Member)
        {
            lock (SaveLockOf(Member.UID))
            {
                // a still queued save of this character (e.g. its logout save) is written first, so it can't undo this
                FlushPendingSaves(Member.UID, null);
                InvalidatePlayerIni(Member.UID);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
                write.Write<uint>("Character", "ClanID", 0);
                write.Write<ushort>("Character", "ClanRank", 0);
                write.Write<uint>("Character", "ClanDonation", 0);
            }
        }
        public static void DestroySpouse(Client.GameClient client)
        {
            uint spouseUID = client.Player.SpouseUID;
            if (spouseUID != 0)
            {
                lock (SaveLockOf(spouseUID))
                {
                    FlushPendingSaves(spouseUID, null);
                    InvalidatePlayerIni(spouseUID);
                    DestroySpouseCore(client);
                }
            }
            else
                DestroySpouseCore(client);
        }
        static void DestroySpouseCore(Client.GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.Write<uint>("Character", "SpouseUID", 0);
                write.WriteString("Character", "Spouse", "None");
                if (client.Player.MyGuild.Members.ContainsKey(client.Player.SpouseUID) && client.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    client.Player.MyGuild.Members[client.Player.SpouseUID].Rank = Role.Flags.GuildMemberRank.Member;
                    write.Write<uint>("Character", "GuildRank", (uint)Role.Flags.GuildMemberRank.Member);
                }
                client.Player.SpouseUID = 0;
            }

            client.Player.Spouse = "None";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Player.SendString(stream,Game.MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
            }
        }
        public static string GenerateDate()
        {
            DateTime now = DateTime.Now;
            return now.Year.ToString() + "and" + now.Month.ToString() + "and" + now.Day.ToString() + " and " + now.Hour.ToString() + " and " + now.Minute.ToString() + " and " + now.Second.ToString();
        }
        public static void UpdateSpouse(Client.GameClient client)
        {
            uint spouseUID = client.Player.SpouseUID;
            if (spouseUID != 0)
            {
                lock (SaveLockOf(spouseUID))
                {
                    FlushPendingSaves(spouseUID, null);
                    InvalidatePlayerIni(spouseUID);
                    WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + spouseUID + ".ini");
                    write.WriteString("Character", "Spouse", client.Player.Name);
                }
            }
        }
        /// <summary>Deletes a player data file together with its .bak/.tmp copies (so a later character
        /// with the same UID can never fall back to the deleted character's data).</summary>
        static void DeletePlayerFile(string path)
        {
            foreach (string f in new string[] { path, path + ".bak", path + ".tmp" })
            {
                try
                {
                    if (File.Exists(f))
                        File.Delete(f);
                }
                catch (Exception e) { Security.Guard.Audit("save-error", 0, "", "delete " + f + ": " + e.Message); }
            }
        }
        /// <summary>
        /// Login queue (one thread): logins, guild/clan/union/spouse updates of offline characters and map records.
        /// Player saves no longer run here: a client queued for a save is handed to the player-save thread
        /// (SaveQueue), so a login never waits behind a save wave. Log strings are forwarded to the Logs writer.
        /// </summary>
        public static ExecuteLogin LoginQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<object>
        {
            public object SynRoot = new object();
            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(object obj)
            {
                Enqueue(obj);
            }
            public override void Enqueue(object obj)
            {
                // Log lines used to be queued here and dropped by OnDequeue; hand them to the async log writer.
                string text = obj as string;
                if (text != null)
                {
                    Logs.OnDequeue(text);
                    return;
                }
                base.Enqueue(obj);
            }
            protected unsafe override void OnDequeue(object obj, int time)
            {
                try
                {
                    if (obj is Role.GameMap)
                    {
                        UpdateMapRace(obj as Role.GameMap);
                    }
                    else if (obj is Role.Instance.Guild.Member)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.Member);
                    }
                    else if (obj is Role.Instance.Guild.UpdateDB)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.UpdateDB);
                    }
                    else if (obj is Role.Instance.Union.Member)
                    {
                        UpdateUnionMember(obj as Role.Instance.Union.Member);
                    }
                    else if (obj is Role.Instance.Clan.Member)
                    {
                        UpdateClanMember(obj as Role.Instance.Clan.Member);
                    }
                    else if (obj is string)
                    {
                        Logs.OnDequeue((string)obj);
                    }
                    else
                    {
                        Client.GameClient client = obj as Client.GameClient;

                        if (client != null && client.Player != null)
                        {
                            if (client.Player.Delete)
                            {
                                // Deleting the character's files runs on the player-save thread, in order with
                                // its saves (a save queued earlier must not recreate the deleted files).
                                QueueUrgentSave(client);
                                return;
                            }

                            if ((client.ClientFlag & Client.ServerFlag.RemoveSpouse) == Client.ServerFlag.RemoveSpouse)
                            {
                                DestroySpouse(client);
                                client.ClientFlag &= ~Client.ServerFlag.RemoveSpouse;
                                return;
                            }
                            if ((client.ClientFlag & Client.ServerFlag.UpdateSpouse) == Client.ServerFlag.UpdateSpouse)
                            {
                                UpdateSpouse(client);
                                client.ClientFlag &= ~Client.ServerFlag.UpdateSpouse;
                                return;
                            }
                            if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation && (client.ClientFlag & Client.ServerFlag.OnLoggion) == Client.ServerFlag.OnLoggion)
                            {
                                // Before this queue was split, a logout save queued before this login always ran
                                // first; keep that: write any pending save of this account now.
                                try
                                {
                                    // client.Key still holds the encrypted id here (HazemShield.RequestLogin stores it as
                                    // received; LoginHandler decrypts it in place), so decrypt it the same way.
                                    FlushPendingSaves(Game.MsgServer.MsgLoginClient.Decrypt(client.Key), client);
                                }
                                catch (Exception e) { MyConsole.SaveException(e); }
                                Game.MsgServer.MsgLoginClient.LoginHandler(client, client.OnLogin);
                            }
                            else if ((client.ClientFlag & Client.ServerFlag.QueuesSave) == Client.ServerFlag.QueuesSave)
                            {
                                // logout saves go before periodic ones
                                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                                    QueueUrgentSave(client);
                                else
                                    QueueSave(client);
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
        }

        /// <summary>Deletes a character's files (keeping dated backups) and removes it from the online pools.
        /// Runs on the player-save thread under the character's save lock.</summary>
        static void DeleteCharacter(Client.GameClient client)
        {
            if (client.Map != null)
                client.Map.View.LeaveMap<Role.Player>(client.Player);

            DateTime Now64 = DateTime.Now;
            if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\BackUp\\Users\\"))
                System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\BackUp\\Users\\");
            if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersSpells\\"))
                System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersSpells\\");
            if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersProfs\\"))
                System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersProfs\\");
            if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersItems\\"))
                System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\BackUp\\PlayersItems\\");
            if (!System.IO.Directory.Exists(Program.ServerConfig.DbLocation + "\\BackUp\\Houses\\"))
                System.IO.Directory.CreateDirectory(Program.ServerConfig.DbLocation + "\\BackUp\\Houses\\");


            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                File.Copy(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini", Program.ServerConfig.DbLocation + "\\BackUp\\Users\\" + client.Player.UID + "date" + GenerateDate() + ".ini", true);
            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersSpells\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersProfs\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin"))
                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersItems\\" + client.Player.UID + "date" + GenerateDate() + ".bin");


            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                File.Delete(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini");
            DeletePlayerFile(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin");// also removes .bak/.tmp when the main file is already gone
            DeletePlayerFile(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin");// also removes .bak/.tmp when the main file is already gone
            DeletePlayerFile(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin");// also removes .bak/.tmp when the main file is already gone

            DeletePlayerFile(Program.ServerConfig.DbLocation + "\\Quests\\" + client.Player.UID + ".bin");// also removes .bak/.tmp when the main file is already gone


            Role.Instance.House house;
            if (client.MyHouse != null && Role.Instance.House.HousePoll.ContainsKey(client.Player.UID))
                Role.Instance.House.HousePoll.TryRemove(client.Player.UID, out house);

            DeletePlayerFile(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin");// also removes .bak/.tmp when the main file is already gone
            Storage.PlayerStorage.OnDeleted(client.Player.UID);// [Storage] its players row + the blobs whose file is gone

            Role.Instance.Chi chi;
            if (Role.Instance.Chi.ChiPool.ContainsKey(client.Player.UID))
            {
                Role.Instance.Chi.ChiPool.TryRemove(client.Player.UID, out chi);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\BackUp\\ChiInfo.txt");
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Dragon", chi.Dragon.ToString());
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Phoenix", chi.Phoenix.ToString());
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Turtle", chi.Turtle.ToString());
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Tiger", chi.Tiger.ToString());

            }
            Role.Instance.Flowers flow;
            if (Role.Instance.Flowers.ClientPoll.ContainsKey(client.Player.UID))
            {
                Role.Instance.Flowers.ClientPoll.TryRemove(client.Player.UID, out flow);
            }

            Role.Instance.InnerPower innerpower;
            if (Role.Instance.InnerPower.InnerPowerPolle.ContainsKey(client.Player.UID))
            {
                Role.Instance.InnerPower.InnerPowerPolle.TryRemove(client.Player.UID, out innerpower);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\BackUp\\InnerPower.txt");
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Dragon", innerpower.ToString());
            }
            Role.Instance.JiangHu Jiang;
            if (Role.Instance.JiangHu.Poll.ContainsKey(client.Player.UID))
            {
                Role.Instance.JiangHu.Poll.TryRemove(client.Player.UID, out Jiang);
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\BackUp\\JiangHuInfo.txt");
                write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Jiang", Jiang.ToString());
            }
            Role.Instance.Associate.MyAsociats Associate;
            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
            {
                Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out Associate);
            }

            Client.GameClient user;
            Server.GamePoll.TryRemove(client.Player.UID, out user);
            if (user != null && user.Player != null)
            {
                lock (Server.NameUsed)
                {
                    if (Server.NameUsed.Contains(user.Player.Name.GetHashCode()))
                        Server.NameUsed.Remove(user.Player.Name.GetHashCode());
                }
            }
        }

        #region player save queue
        // Player saves run on their own thread ("PlayerSaveQueue"):
        //  * one queued save per client at a time (requests while one is queued are merged; the flag is cleared
        //    when the save starts, so a request made during a save queues a new one);
        //  * logout saves (Disconnect flag), character deletion and saves requested by packet handlers go to the
        //    urgent queue, periodic saves to the normal one; the urgent queue is always served first;
        //  * a save and a load of the same UID never overlap (SaveLocks), and a login first writes any save of
        //    that UID that is still pending (FlushPendingSaves), so the load always sees the newest data;
        //  * the logout save removes the client from GamePoll after its files are written (as before).

        sealed class SaveState
        {
            public bool Queued;          // a save was requested and has not started yet (lock(this))
            public bool InUrgentQueue;   // an entry for this client waits in the urgent queue (lock(this))
            public long NextPeriodicMs;  // periodic save schedule (KernelThread only)
            public Dictionary<string, string> IniCache; // last value written per key (under SaveLocks)
            public int IniGeneration;    // ExternalIniWrites value when IniCache was filled
            public int DeltaSaves;       // delta saves since the last full write
        }

        static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Client.GameClient, SaveState> saveStates =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Client.GameClient, SaveState>();
        static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Client.GameClient, SaveState>.CreateValueCallback newSaveState =
            k => new SaveState();
        static readonly System.Collections.Concurrent.ConcurrentDictionary<Client.GameClient, byte> PendingSaves =
            new System.Collections.Concurrent.ConcurrentDictionary<Client.GameClient, byte>();
        // bumped whenever something other than SaveClient writes a character's ini file
        static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, int> ExternalIniWrites =
            new System.Collections.Concurrent.ConcurrentDictionary<uint, int>();

        static SaveState StateOf(Client.GameClient client)
        {
            return saveStates.GetValue(client, newSaveState);
        }

        public static readonly PlayerSaveQueue SaveQueue = new PlayerSaveQueue();

        /// <summary>Queues a save of the character on the player-save thread (merged with one already queued).
        /// Returns true if a new queue entry was added.</summary>
        public static bool QueueSave(Client.GameClient client)
        {
            return RequestSave(client, false);
        }
        /// <summary>Same as QueueSave, but served before periodic saves (logout, deletion, important changes).</summary>
        public static bool QueueUrgentSave(Client.GameClient client)
        {
            return RequestSave(client, true);
        }
        static bool RequestSave(Client.GameClient client, bool urgent)
        {
            if (client == null || client.Player == null)
                return false;
            SaveState st = StateOf(client);
            bool addNormal = false, addUrgent = false;
            lock (st)
            {
                if (urgent)
                {
                    if (!st.InUrgentQueue)
                    {
                        st.InUrgentQueue = true;
                        addUrgent = true;
                    }
                }
                else if (!st.Queued)
                    addNormal = true;
                st.Queued = true;
                PendingSaves[client] = 0;
            }
            if (addUrgent)
                SaveQueue.Add(client, true);
            else if (addNormal)
                SaveQueue.Add(client, false);
            return addUrgent || addNormal;
        }

        /// <summary>Runs the queued save of this client if it has not run yet (player-save thread, or a login
        /// flushing a pending save).</summary>
        static void RunQueuedSave(Client.GameClient client, bool fromUrgentQueue)
        {
            SaveState st = StateOf(client);
            if (client.Player == null)
            {
                byte ignored;
                lock (st)
                {
                    if (fromUrgentQueue)
                        st.InUrgentQueue = false;
                    st.Queued = false;
                }
                PendingSaves.TryRemove(client, out ignored);
                return;
            }
            object saveLock = SaveLockOf(client.Player.UID);
            System.Threading.Interlocked.Increment(ref SavesInFlight);
            try
            {
                lock (saveLock)
                {
                    bool run;
                    lock (st)
                    {
                        if (fromUrgentQueue)
                            st.InUrgentQueue = false;
                        run = st.Queued;
                        st.Queued = false;
                        if (run)
                        {
                            byte ignored;
                            PendingSaves.TryRemove(client, out ignored);
                        }
                    }
                    if (run)
                        ExecuteSaveItem(client);
                }
            }
            finally
            {
                System.Threading.Interlocked.Decrement(ref SavesInFlight);
            }
        }

        static void ExecuteSaveItem(Client.GameClient client)
        {
            if (client.Player.Delete)
            {
                DeleteCharacter(client);
                return;
            }
            if (client.Player.OnTransform)
            {
                client.Player.HitPoints = Math.Min(client.Player.HitPoints, (int)client.Status.MaxHitpoints);
            }
            SaveClientCore(client);
        }

        /// <summary>Writes, on the calling thread, every save of this UID that is still waiting in the save queue
        /// (except the given client's). Used before a login so it never loads older data.</summary>
        public static void FlushPendingSaves(uint uid, Client.GameClient except)
        {
            if (uid == 0 || PendingSaves.IsEmpty)
                return;
            foreach (var pending in PendingSaves)
            {
                Client.GameClient other = pending.Key;
                if (other == null || other == except || other.Player == null || other.Player.UID != uid)
                    continue;
                try
                {
                    RunQueuedSave(other, false);
                }
                catch (Exception e) { MyConsole.SaveException(e); }
            }
        }

        /// <summary>Call after writing a character's Users\uid.ini from anywhere but SaveClient: the next save of
        /// that character (if online) then rewrites every key instead of only the changed ones.</summary>
        public static void InvalidatePlayerIni(uint uid)
        {
            ExternalIniWrites.AddOrUpdate(uid, 1, (k, v) => unchecked(v + 1));
        }

        /// <summary>Waits until the login queue and the player-save queue are empty and no save is running.</summary>
        public static bool FinishPlayerSaves(int timeoutMs)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Func<int> left = () => timeoutMs < 0 ? -1 : (int)Math.Max(0, timeoutMs - watch.ElapsedMilliseconds);
            // twice: a login-queue item can queue a save, a save never queues login work
            for (int round = 0; round < 2; round++)
            {
                if (!LoginQueue.Finish(left()))
                    return false;
                if (!SaveQueue.Finish(left()))
                    return false;
            }
            while (SavesRunning > 0)
            {
                if (timeoutMs >= 0 && watch.ElapsedMilliseconds >= timeoutMs)
                    return false;
                System.Threading.Thread.Sleep(1);
            }
            return true;
        }

        static readonly System.Diagnostics.Stopwatch saveClock = System.Diagnostics.Stopwatch.StartNew();
        static int playerSaveIntervalSec;
        /// <summary>shell.ini [Performance] PlayerSaveIntervalSec (default 250, min 30): seconds between two
        /// periodic saves of the same online character.</summary>
        public static int PlayerSaveIntervalSec
        {
            get
            {
                int v = playerSaveIntervalSec;
                if (v == 0)
                {
                    v = Math.Max(30, Conquer6609.Framework.Settings.Int("Performance", "PlayerSaveIntervalSec", 250));
                    playerSaveIntervalSec = v;
                }
                return v;
            }
        }

        /// <summary>Periodic saves, spread out (called every KernelThread tick): an online character is queued
        /// when its last periodic save is PlayerSaveIntervalSec old. The first one is at login + (UID % interval) + 1 s,
        /// so the saves of all players are spread over the interval instead of one wave.</summary>
        public static int QueueDuePlayerSaves()
        {
            int interval = PlayerSaveIntervalSec;
            long now = saveClock.ElapsedMilliseconds;
            int queued = 0;
            foreach (var entry in Server.GamePoll)
            {
                Client.GameClient user = entry.Value;
                if (user == null || user.Player == null || user.OnInterServer)
                    continue;
                if ((user.ClientFlag & Client.ServerFlag.LoginFull) != Client.ServerFlag.LoginFull)
                    continue;
                SaveState st = StateOf(user);
                if (st.NextPeriodicMs == 0)
                {
                    st.NextPeriodicMs = now + ((long)(user.Player.UID % (uint)interval) + 1) * 1000;
                    continue;
                }
                if (now < st.NextPeriodicMs)
                    continue;
                st.NextPeriodicMs = now + interval * 1000L;
                user.ClientFlag |= Client.ServerFlag.QueuesSave;
                if (QueueSave(user))
                    queued++;
            }
            return queued;
        }

        /// <summary>Queues a save of every fully logged-in character (old wave behaviour, console "save").</summary>
        public static int QueueAllPlayerSaves(bool urgent)
        {
            int queued = 0;
            foreach (var entry in Server.GamePoll)
            {
                Client.GameClient user = entry.Value;
                if (user == null || user.Player == null || user.OnInterServer)
                    continue;
                if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                {
                    user.ClientFlag |= Client.ServerFlag.QueuesSave;
                    if (urgent ? QueueUrgentSave(user) : QueueSave(user))
                        queued++;
                }
            }
            return queued;
        }

        /// <summary>The player-save thread: urgent entries first, then periodic ones.</summary>
        public sealed class PlayerSaveQueue
        {
            readonly System.Collections.Concurrent.ConcurrentQueue<Client.GameClient> urgent = new System.Collections.Concurrent.ConcurrentQueue<Client.GameClient>();
            readonly System.Collections.Concurrent.ConcurrentQueue<Client.GameClient> normal = new System.Collections.Concurrent.ConcurrentQueue<Client.GameClient>();
            readonly System.Threading.AutoResetEvent signal = new System.Threading.AutoResetEvent(false);
            readonly object startLock = new object();
            System.Threading.Thread thread;
            int inFlight;// entries dequeued but not finished yet

            public int Count { get { return urgent.Count + normal.Count; } }
            public int UrgentCount { get { return urgent.Count; } }

            internal void Add(Client.GameClient client, bool isUrgent)
            {
                EnsureStarted();
                if (isUrgent)
                    urgent.Enqueue(client);
                else
                    normal.Enqueue(client);
                signal.Set();
            }

            void EnsureStarted()
            {
                if (thread != null)
                    return;
                lock (startLock)
                {
                    if (thread != null)
                        return;
                    var t = new System.Threading.Thread(Work);
                    t.IsBackground = true;
                    t.Name = "PlayerSaveQueue";
                    t.Start();
                    thread = t;
                }
            }

            void Work()
            {
                while (true)
                {
                    Client.GameClient client = null;
                    // counted before the dequeue so Finish() never sees "empty queue, nothing running" in between
                    System.Threading.Interlocked.Increment(ref inFlight);
                    try
                    {
                        bool fromUrgent = urgent.TryDequeue(out client);
                        if (!fromUrgent && !normal.TryDequeue(out client))
                            client = null;
                        if (client != null)
                        {
                            try
                            {
                                RunQueuedSave(client, fromUrgent);
                            }
                            catch (Exception e) { MyConsole.SaveException(e); }
                        }
                    }
                    catch (Exception e) { MyConsole.SaveException(e); }
                    finally
                    {
                        System.Threading.Interlocked.Decrement(ref inFlight);
                    }
                    if (client == null)
                        signal.WaitOne(1000);
                }
            }

            /// <summary>Waits until both queues are empty and the save being written (if any) is done.
            /// timeoutMs &lt; 0 waits forever; returns false if work was still pending when it expired.</summary>
            public bool Finish(int timeoutMs)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                while (urgent.Count > 0 || normal.Count > 0 || System.Threading.Volatile.Read(ref inFlight) > 0)
                {
                    if (timeoutMs >= 0 && watch.ElapsedMilliseconds >= timeoutMs)
                        return false;
                    System.Threading.Thread.Sleep(1);
                }
                return true;
            }
        }

        /// <summary>
        /// Writes the [Character] keys of Users\uid.ini through WritePrivateProfileString, like IniFile did, but
        /// skips keys whose value is the same as the one this session last wrote successfully (each call rewrites
        /// the whole file). A full write (every key) is done on the first save of a session, on the logout save,
        /// when the file is missing, after another writer touched the file (InvalidatePlayerIni), every
        /// [Performance] PlayerIniFullSaveEvery-th save (default 12) and always when PlayerIniDeltaSave=0.
        /// The file format is unchanged.
        /// </summary>
        sealed class PlayerIniWriter
        {
            static int deltaMode = -1, fullEvery = -1;
            readonly string file;
            readonly Dictionary<string, string> cache;
            readonly bool full;
            public int Calls, Skipped;

            public PlayerIniWriter(string file, SaveState st, uint uid, bool finalSave)
            {
                this.file = file;
                if (deltaMode < 0)
                    deltaMode = Conquer6609.Framework.Settings.Bool("Performance", "PlayerIniDeltaSave", true) ? 1 : 0;
                if (fullEvery < 0)
                    fullEvery = Math.Max(0, Conquer6609.Framework.Settings.Int("Performance", "PlayerIniFullSaveEvery", 12));
                int generation;
                ExternalIniWrites.TryGetValue(uid, out generation);
                full = deltaMode == 0 || finalSave || st.IniCache == null || st.IniGeneration != generation
                    || (fullEvery > 0 && st.DeltaSaves >= fullEvery) || !File.Exists(file);
                if (full)
                {
                    st.IniCache = new Dictionary<string, string>(256, StringComparer.Ordinal);
                    st.IniGeneration = generation;
                    st.DeltaSaves = 0;
                }
                else
                    st.DeltaSaves++;
                cache = st.IniCache;
            }

            public void Write<T>(string Section, string Key, T Value)
            {
                Put(Section, Key, Value.ToString());// same conversion as IniFile.Write<T>
            }
            public void WriteString(string Section, string Key, string Value)
            {
                if (Value == null)
                    Value = "0";// same as IniFile.WriteString
                Put(Section, Key, Value);
            }
            void Put(string section, string key, string value)
            {
                string cacheKey = section + "\u0001" + key;
                string last;
                if (!full && cache.TryGetValue(cacheKey, out last) && string.Equals(last, value, StringComparison.Ordinal))
                {
                    Skipped++;
                    return;
                }
                Calls++;
                if (WindowsAPI.Kernel32.WritePrivateProfileString(section, key, value, file) != 0)
                    cache[cacheKey] = value;
                else
                    cache.Remove(cacheKey);
            }
        }
        #endregion
       
    }
}
