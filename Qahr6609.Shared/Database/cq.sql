-- Qahr6609: schema of the MySQL account database (tables only). Player/account rows were removed for the public repo;
-- only the game seed rows of `panelattack` (skill damage multipliers, read at start-up) and `configuration` are kept.
-- Create it (name = HazemConfig.ini MySqlDatabase, e.g. mega):  mysql -u root -p -e "CREATE DATABASE mega"  then  mysql -u root -p mega < cq.sql
/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50505
Source Host           : localhost:3306
Source Database       : mega

Target Server Type    : MYSQL
Target Server Version : 50505
File Encoding         : 65001

Date: 2026-02-19 17:38:36
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `accounts`
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `EntityID` bigint(255) NOT NULL AUTO_INCREMENT,
  `Username` char(25) NOT NULL DEFAULT '',
  `Password` varchar(16) DEFAULT '',
  `EarthID` varchar(16) DEFAULT NULL,
  `Email` varchar(100) DEFAULT '',
  `State` bigint(5) DEFAULT NULL,
  `Question` varchar(100) DEFAULT NULL,
  `answer` varchar(30) DEFAULT NULL,
  `Country` varchar(110) DEFAULT NULL,
  `City` varchar(100) DEFAULT NULL,
  `IP` varchar(100) DEFAULT NULL,
  `phone` varchar(15) DEFAULT NULL,
  `Online` tinyint(2) DEFAULT 0,
  `Name` varchar(16) DEFAULT '',
  `macs` varchar(220) DEFAULT NULL,
  `BannedID` bigint(5) DEFAULT 2,
  `MobileNumber` varchar(100) DEFAULT NULL,
  `BanReason` varchar(255) NOT NULL DEFAULT '',
  `secretPIN` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Username`) USING BTREE,
  UNIQUE KEY `f43f43` (`EntityID`) USING BTREE,
  KEY `ggg` (`Username`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=10212287 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `admin`
-- ----------------------------
DROP TABLE IF EXISTS `admin`;
CREATE TABLE `admin` (
  `user_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(30) DEFAULT NULL,
  `user_name` varchar(50) DEFAULT NULL,
  `pass` varchar(100) DEFAULT NULL,
  `status` enum('admin') NOT NULL,
  PRIMARY KEY (`user_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `arena`
-- ----------------------------
DROP TABLE IF EXISTS `arena`;
CREATE TABLE `arena` (
  `EntityID` int(10) unsigned NOT NULL DEFAULT 0,
  `EntityName` char(45) NOT NULL DEFAULT '',
  `CurrentHonor` int(10) unsigned DEFAULT 0,
  `Level` int(10) unsigned DEFAULT 0,
  `Class` int(10) unsigned DEFAULT 0
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=FIXED;

-- ----------------------------
-- Table structure for `background`
-- ----------------------------
DROP TABLE IF EXISTS `background`;
CREATE TABLE `background` (
  `background_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `image` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`background_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `bandmacid`
-- ----------------------------
DROP TABLE IF EXISTS `bandmacid`;
CREATE TABLE `bandmacid` (
  `MacID` varchar(16) NOT NULL DEFAULT '0000000000000000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `banned`
-- ----------------------------
DROP TABLE IF EXISTS `banned`;
CREATE TABLE `banned` (
  `UID` varchar(16) NOT NULL,
  `username` varchar(16) NOT NULL DEFAULT '',
  `Hours` bigint(18) unsigned NOT NULL DEFAULT 0,
  `StartBan` bigint(255) unsigned NOT NULL DEFAULT 0,
  `Reason` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`UID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `bannedmac`
-- ----------------------------
DROP TABLE IF EXISTS `bannedmac`;
CREATE TABLE `bannedmac` (
  `username` varchar(16) NOT NULL,
  `MacID` varchar(16) NOT NULL DEFAULT '0000000000000000',
  PRIMARY KEY (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `banner`
-- ----------------------------
DROP TABLE IF EXISTS `banner`;
CREATE TABLE `banner` (
  `banner_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `image` varchar(110) DEFAULT NULL,
  `title` varchar(150) DEFAULT NULL,
  `s_title` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`banner_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `comments`
-- ----------------------------
DROP TABLE IF EXISTS `comments`;
CREATE TABLE `comments` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `postid` int(30) NOT NULL,
  `comment` text NOT NULL,
  `user` varchar(30) NOT NULL,
  `date` varchar(30) NOT NULL,
  `writerid` int(255) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `configuration`
-- ----------------------------
DROP TABLE IF EXISTS `configuration`;
CREATE TABLE `configuration` (
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `OnlineCount` int(16) DEFAULT NULL,
  `MaxOnline` int(16) DEFAULT NULL,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of configuration
-- ----------------------------
INSERT INTO `configuration` VALUES ('EpicConquer', '0', '0');

-- ----------------------------
-- Table structure for `cpanal`
-- ----------------------------
DROP TABLE IF EXISTS `cpanal`;
CREATE TABLE `cpanal` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Website` varchar(255) DEFAULT NULL,
  `Title` text DEFAULT NULL,
  `URL` varchar(255) DEFAULT NULL,
  `VERSION` bigint(20) DEFAULT NULL,
  `Drop` bigint(20) DEFAULT NULL,
  `Max_Level` bigint(20) DEFAULT NULL,
  `Max_Plus` bigint(20) DEFAULT NULL,
  `LATEST_VIDEOS` text DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `cpanel`
-- ----------------------------
DROP TABLE IF EXISTS `cpanel`;
CREATE TABLE `cpanel` (
  `Website_Name` varchar(18) NOT NULL,
  `Website_url` text DEFAULT NULL,
  `Domain` text DEFAULT NULL,
  `date` varchar(255) DEFAULT NULL,
  `Time` varchar(255) DEFAULT NULL,
  `Email` text DEFAULT NULL,
  `password` text DEFAULT NULL,
  `Host` text DEFAULT NULL,
  `Port` text DEFAULT NULL,
  `SMTPSecure` text DEFAULT NULL,
  `mate` longtext DEFAULT NULL,
  `sitemap` varchar(255) DEFAULT NULL,
  `GM` bigint(20) DEFAULT NULL,
  `stylee` longtext NOT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Transfer` varchar(255) DEFAULT NULL,
  `version` varchar(255) DEFAULT NULL,
  `max_execution` bigint(20) DEFAULT NULL,
  `King` bigint(20) DEFAULT NULL,
  `prince` bigint(20) DEFAULT NULL,
  `Doke` bigint(20) DEFAULT NULL,
  `limits` bigint(20) DEFAULT NULL,
  `boy` bigint(20) DEFAULT NULL,
  `girl` bigint(20) DEFAULT NULL,
  `language` varchar(255) DEFAULT NULL,
  `link_paypal` varchar(255) DEFAULT NULL,
  `email_paypal` varchar(255) DEFAULT NULL,
  `logo` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `cps_rank`
-- ----------------------------
DROP TABLE IF EXISTS `cps_rank`;
CREATE TABLE `cps_rank` (
  `UID` bigint(16) unsigned DEFAULT NULL,
  `Name` varchar(32) DEFAULT NULL,
  `cps` bigint(16) DEFAULT NULL,
  `Class` bigint(16) DEFAULT NULL,
  `Level` bigint(16) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `ctfranks`
-- ----------------------------
DROP TABLE IF EXISTS `ctfranks`;
CREATE TABLE `ctfranks` (
  `UID` bigint(16) DEFAULT NULL,
  `GuildName` varchar(25) DEFAULT NULL,
  `GuildLeader` varchar(25) DEFAULT NULL,
  `CTFPoints` bigint(16) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `donation_paypal`
-- ----------------------------
DROP TABLE IF EXISTS `donation_paypal`;
CREATE TABLE `donation_paypal` (
  `id_pay` int(5) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `time` datetime NOT NULL,
  `account_name` varchar(60) NOT NULL,
  `pachet_id` varchar(255) NOT NULL,
  `sv_id` varchar(255) NOT NULL,
  `successed` varchar(255) NOT NULL,
  PRIMARY KEY (`id_pay`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=2139263 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `download`
-- ----------------------------
DROP TABLE IF EXISTS `download`;
CREATE TABLE `download` (
  `download_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `from` varchar(50) DEFAULT NULL,
  `url` varchar(100) DEFAULT NULL,
  `type` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`download_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `entities`
-- ----------------------------
DROP TABLE IF EXISTS `entities`;
CREATE TABLE `entities` (
  `UID` int(11) NOT NULL,
  `Name` varchar(32) NOT NULL DEFAULT '',
  `Class` int(11) NOT NULL DEFAULT 0,
  `Level` int(11) NOT NULL DEFAULT 1,
  `Nobility` bigint(20) NOT NULL DEFAULT 0,
  `Owner` varchar(32) NOT NULL DEFAULT '',
  `VipLevel` tinyint(4) NOT NULL DEFAULT 0,
  `Money` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `event`
-- ----------------------------
DROP TABLE IF EXISTS `event`;
CREATE TABLE `event` (
  `event_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`event_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `events`
-- ----------------------------
DROP TABLE IF EXISTS `events`;
CREATE TABLE `events` (
  `id` int(12) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `Username` varchar(25) DEFAULT NULL,
  `title` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `image_url` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- ----------------------------
-- Table structure for `guide`
-- ----------------------------
DROP TABLE IF EXISTS `guide`;
CREATE TABLE `guide` (
  `guide_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(50) DEFAULT NULL,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`guide_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `guide1`
-- ----------------------------
DROP TABLE IF EXISTS `guide1`;
CREATE TABLE `guide1` (
  `guide_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(50) DEFAULT NULL,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`guide_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `guide2`
-- ----------------------------
DROP TABLE IF EXISTS `guide2`;
CREATE TABLE `guide2` (
  `guide_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(50) DEFAULT NULL,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`guide_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `guide3`
-- ----------------------------
DROP TABLE IF EXISTS `guide3`;
CREATE TABLE `guide3` (
  `guide_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(50) DEFAULT NULL,
  `description` text DEFAULT NULL,
  PRIMARY KEY (`guide_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=14 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `guilds`
-- ----------------------------
DROP TABLE IF EXISTS `guilds`;
CREATE TABLE `guilds` (
  `UID` int(10) unsigned NOT NULL DEFAULT 0,
  `Name` varchar(45) NOT NULL DEFAULT '',
  `Wins` bigint(255) unsigned DEFAULT 0,
  `LeaderName` varchar(45) NOT NULL DEFAULT '0',
  `WinSuperGW` bigint(16) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `inbox`
-- ----------------------------
DROP TABLE IF EXISTS `inbox`;
CREATE TABLE `inbox` (
  `Username` longtext DEFAULT NULL,
  `Email` longtext DEFAULT NULL,
  `Message` longtext DEFAULT NULL,
  `Time` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `information`
-- ----------------------------
DROP TABLE IF EXISTS `information`;
CREATE TABLE `information` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `text` text DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `joins`
-- ----------------------------
DROP TABLE IF EXISTS `joins`;
CREATE TABLE `joins` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `ip` varchar(45) NOT NULL,
  `joined_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=164 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- ----------------------------
-- Table structure for `keygift`
-- ----------------------------
DROP TABLE IF EXISTS `keygift`;
CREATE TABLE `keygift` (
  `ID` bigint(255) NOT NULL AUTO_INCREMENT,
  `Keygenerate` varchar(22) NOT NULL DEFAULT '',
  UNIQUE KEY `cap` (`ID`) USING BTREE,
  KEY `asd` (`Keygenerate`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=1016899 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `likes`
-- ----------------------------
DROP TABLE IF EXISTS `likes`;
CREATE TABLE `likes` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `postid` int(11) NOT NULL,
  `user` varchar(30) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `log_payments`
-- ----------------------------
DROP TABLE IF EXISTS `log_payments`;
CREATE TABLE `log_payments` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `user` varchar(255) NOT NULL,
  `alert` text NOT NULL,
  `IP` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `logo`
-- ----------------------------
DROP TABLE IF EXISTS `logo`;
CREATE TABLE `logo` (
  `logo_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `image` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`logo_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `macs`
-- ----------------------------
DROP TABLE IF EXISTS `macs`;
CREATE TABLE `macs` (
  `mac` varchar(255) NOT NULL,
  `id` bigint(20) NOT NULL,
  `Claims` bigint(11) NOT NULL DEFAULT 0,
  `Online` bigint(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`mac`,`id`,`Claims`,`Online`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `mrmohamed`
-- ----------------------------
DROP TABLE IF EXISTS `mrmohamed`;
CREATE TABLE `mrmohamed` (
  `UID` bigint(18) unsigned NOT NULL DEFAULT 0,
  `Owner` varchar(45) NOT NULL DEFAULT '',
  `Name` varchar(16) NOT NULL DEFAULT '',
  `ConquerPoints` bigint(18) unsigned DEFAULT 0,
  `Money` bigint(18) unsigned DEFAULT 100,
  `Class` tinyint(5) unsigned NOT NULL DEFAULT 10,
  `FirstCreditPoints` bigint(18) NOT NULL DEFAULT 0,
  `TQPoint` bigint(18) NOT NULL,
  `ConquerPointsFake` bigint(18) NOT NULL DEFAULT 0,
  `Level` tinyint(5) unsigned DEFAULT 1,
  `VIPLevel` tinyint(5) unsigned NOT NULL DEFAULT 3,
  `BoundCps` bigint(18) unsigned DEFAULT 2000,
  `Face` smallint(12) unsigned NOT NULL DEFAULT 0,
  `Body` smallint(12) unsigned NOT NULL DEFAULT 0,
  `HairStyle` smallint(12) unsigned DEFAULT 21,
  `Online` tinyint(2) unsigned DEFAULT 0,
  `Strength` smallint(12) unsigned DEFAULT 1,
  `Agility` smallint(12) unsigned DEFAULT 1,
  `Vitality` smallint(12) unsigned DEFAULT 1,
  `Spirit` smallint(12) unsigned DEFAULT 0,
  `Atributes` smallint(12) unsigned DEFAULT 0,
  `Hitpoints` mediumint(16) unsigned DEFAULT 93,
  `Mana` mediumint(16) unsigned DEFAULT 0,
  `MapID` smallint(12) unsigned DEFAULT 1002,
  `X` smallint(12) unsigned DEFAULT 271,
  `Y` smallint(12) unsigned DEFAULT 421,
  `PKPoints` smallint(12) unsigned DEFAULT 0,
  `Experience` bigint(255) unsigned DEFAULT 0,
  `QuizPoints` mediumint(30) unsigned DEFAULT 0,
  `PreviousMapID` smallint(12) unsigned DEFAULT 1010,
  `Reborn` tinyint(2) unsigned DEFAULT 0,
  `1stClass` tinyint(5) unsigned DEFAULT 10,
  `2ndClass` tinyint(2) unsigned DEFAULT 0,
  `3rdClass` tinyint(2) unsigned DEFAULT 0,
  `Spouse` varchar(16) DEFAULT 'None',
  `WarehousePW` varchar(16) DEFAULT '',
  `MoneySave` bigint(255) unsigned DEFAULT 0,
  `FirstRebornClass` tinyint(5) unsigned DEFAULT 0,
  `SecondRebornClass` tinyint(5) unsigned DEFAULT 0,
  `FirstRebornLevel` tinyint(5) unsigned DEFAULT 0,
  `SecondRebornLevel` tinyint(5) unsigned DEFAULT 0,
  `EnlightenPoints` bigint(18) unsigned DEFAULT 0,
  `DoubleExpTime` bigint(18) unsigned DEFAULT 0,
  `gwkill` bigint(18) unsigned DEFAULT 0,
  `HeavenBlessingTime` bigint(255) unsigned DEFAULT 0,
  `BlessTime` bigint(18) unsigned DEFAULT 0,
  `LastDragonBallUse` bigint(255) DEFAULT 0,
  `LastResetTime` bigint(255) DEFAULT 0,
  `EnlightsReceived` tinyint(5) unsigned DEFAULT 0,
  `EnlightmentWait` mediumint(100) unsigned DEFAULT 0,
  `DoubleExpToday` tinyint(5) unsigned DEFAULT 0,
  `GuildID` bigint(18) unsigned DEFAULT 0,
  `GuildRank` bigint(18) unsigned DEFAULT 0,
  `GuildSilverDonation` bigint(255) unsigned DEFAULT 0,
  `GuildConquerPointDonation` bigint(255) unsigned DEFAULT 0,
  `VirtuePoints` bigint(255) unsigned DEFAULT 0,
  `HeadgearClaim` tinyint(5) unsigned DEFAULT 0,
  `NecklaceClaim` tinyint(5) unsigned DEFAULT 0,
  `ArmorClaim` tinyint(5) unsigned DEFAULT 0,
  `WeaponClaim` tinyint(5) unsigned DEFAULT 0,
  `BootsClaim` tinyint(5) unsigned DEFAULT 0,
  `TowerClaim` tinyint(5) unsigned DEFAULT 0,
  `FanClaim` tinyint(5) unsigned DEFAULT 0,
  `RingClaim` tinyint(5) unsigned DEFAULT 0,
  `VendingDisguise` smallint(5) DEFAULT 0,
  `ChatBanTime` bigint(255) DEFAULT 0,
  `ChatBanLasts` bigint(255) DEFAULT 0,
  `ChatBanned` tinyint(5) DEFAULT 0,
  `InLottery` tinyint(5) DEFAULT 0,
  `LotteryEntries` mediumint(10) DEFAULT 0,
  `LastLotteryEntry` bigint(255) DEFAULT 0,
  `PreviousX` mediumint(10) DEFAULT 0,
  `PreviousY` mediumint(10) DEFAULT 0,
  `OfflineTGEnterTime` bigint(255) DEFAULT 0,
  `ExpBalls` mediumint(10) DEFAULT 0,
  `Status` bigint(18) DEFAULT 0,
  `Status2` bigint(18) DEFAULT 0,
  `UnionID` bigint(18) unsigned DEFAULT 0,
  `ClanId` int(36) unsigned DEFAULT 0,
  `ClanDonation` bigint(255) unsigned DEFAULT 0,
  `ClanRank` int(36) unsigned DEFAULT 0,
  `SubPro` int(36) unsigned NOT NULL DEFAULT 0,
  `SubProLevel` int(36) unsigned NOT NULL DEFAULT 0,
  `StudyPoints` int(36) unsigned DEFAULT 0,
  `LastLogin` bigint(16) DEFAULT 0,
  `My_Title` int(36) NOT NULL DEFAULT 0,
  `Status3` bigint(32) NOT NULL DEFAULT 0,
  `Status4` bigint(32) NOT NULL DEFAULT 0,
  `Quest` bigint(32) NOT NULL DEFAULT 0,
  `Flower` bigint(32) NOT NULL DEFAULT 0,
  `namechange` varchar(16) DEFAULT NULL,
  `CountryID` smallint(4) DEFAULT NULL,
  `spiritquestbead` int(4) NOT NULL DEFAULT 0,
  `canacceptspiritbead` int(1) NOT NULL DEFAULT 0,
  `collectedspirits` int(4) NOT NULL DEFAULT 0,
  `EditCount` smallint(12) DEFAULT 0,
  `EditAllowed` smallint(12) NOT NULL DEFAULT 1,
  `claimed` tinyint(2) NOT NULL DEFAULT 0,
  `RacePoints` bigint(18) NOT NULL DEFAULT 0,
  `TotalDoantionGuild` bigint(255) unsigned NOT NULL DEFAULT 0,
  `ArsenalDonation` bigint(255) unsigned NOT NULL DEFAULT 0,
  `OnlinePoints` bigint(18) NOT NULL DEFAULT 0,
  `LuckyPoints` int(4) NOT NULL DEFAULT 0,
  `updatelist` bigint(32) DEFAULT NULL,
  `Exploits` bigint(255) DEFAULT NULL,
  `ClaimedTeamPK` tinyint(2) NOT NULL,
  `ClaimedSTeamPK` tinyint(2) NOT NULL,
  `GuildLilies` bigint(255) DEFAULT NULL,
  `GuildRouses` bigint(255) DEFAULT NULL,
  `GuildOrchids` bigint(255) DEFAULT NULL,
  `GuildTulips` bigint(255) DEFAULT NULL,
  `GuildPkDonation` bigint(255) DEFAULT NULL,
  `CTFCpsReward` bigint(255) DEFAULT NULL,
  `CTFSilverReward` bigint(255) DEFAULT NULL,
  `joinkingtime` bigint(255) DEFAULT NULL,
  `Kingfight` bigint(255) DEFAULT NULL,
  `kingwin` bigint(255) DEFAULT NULL,
  `OnlineTrainning` bigint(18) DEFAULT NULL,
  `HuntingExp` bigint(18) DEFAULT NULL,
  `ForgetPassword` bigint(32) DEFAULT NULL,
  `VoteSystem` bigint(32) DEFAULT NULL,
  `FirstCredit` tinyint(2) NOT NULL,
  `ClaimedElitePk` tinyint(2) NOT NULL,
  `CursedTime` bigint(255) NOT NULL,
  `SwordSoul` bigint(32) NOT NULL,
  `BansheeSpirit` bigint(32) NOT NULL,
  `ProtectionTime` bigint(18) DEFAULT NULL,
  `ClaimedExp` bigint(32) NOT NULL,
  `Energy` bigint(255) DEFAULT NULL,
  `ExtraInventory` bigint(255) DEFAULT NULL,
  `Appearance` tinyint(5) NOT NULL,
  `JiangSettings` tinyint(5) NOT NULL,
  `strikePoints` bigint(16) DEFAULT NULL,
  `lacb` bigint(16) DEFAULT NULL,
  `TitlePoints` bigint(20) DEFAULT 0,
  `ChampionPoints` bigint(16) NOT NULL DEFAULT 0,
  `Offical` smallint(12) unsigned DEFAULT 0,
  `Harem` smallint(12) unsigned DEFAULT 0,
  `Guards` smallint(12) unsigned DEFAULT 0,
  `UnionExploits` bigint(18) unsigned DEFAULT 0,
  `TempestWings` int(12) NOT NULL,
  `InnerBook` bigint(18) NOT NULL DEFAULT 0,
  `ProsbertyPack` bigint(18) NOT NULL DEFAULT 0,
  `Champketos` bigint(18) NOT NULL DEFAULT 0,
  `EpicTimes` bigint(18) DEFAULT 5,
  `NemisisPoint` bigint(18) DEFAULT 0,
  `EpicMonkPrize` bigint(18) DEFAULT 27,
  `EpicMonkTimes` bigint(18) DEFAULT 1,
  `TrojanEpicOn` tinyint(5) DEFAULT NULL,
  `Epictrue` tinyint(5) DEFAULT NULL,
  `FirstStagetrojan` bigint(18) DEFAULT 0,
  `Stars` bigint(18) DEFAULT 0,
  `ewing` bigint(18) DEFAULT 0,
  `etitle` bigint(18) DEFAULT 0,
  `Windwalker` bigint(18) NOT NULL DEFAULT 0,
  `Wardrobe` varchar(255) NOT NULL,
  `BlackList` longtext NOT NULL,
  `DailySignedDays` longtext NOT NULL,
  `DailySignRewards` bigint(255) NOT NULL DEFAULT 0,
  `DailySignVIPChances` bigint(255) NOT NULL DEFAULT 10,
  `TodayChangedBranch` bigint(18) NOT NULL DEFAULT 0,
  `Prestige` bigint(255) NOT NULL DEFAULT 0,
  `Kingdomquiz` int(40) NOT NULL,
  `DivineRXP` bigint(18) NOT NULL DEFAULT 0,
  `VIPLevelFake` bigint(18) DEFAULT 0,
  `VipStamp` bigint(255) DEFAULT 0,
  `VIPLevelParment` bigint(18) NOT NULL DEFAULT 0,
  `DidBrightFortune` bigint(255) NOT NULL DEFAULT 0,
  `DonefurtuneLastStage` bigint(255) NOT NULL DEFAULT 0,
  `Donefurtunefirststagege` bigint(255) NOT NULL DEFAULT 0,
  `TransferedMessage` bigint(20) NOT NULL DEFAULT 0,
  `winnerctf` bigint(255) NOT NULL DEFAULT 0,
  `s` varchar(16) DEFAULT '',
  `DidFirstHeavenTreasury` int(40) DEFAULT 0,
  `MystreyFruit` smallint(12) NOT NULL DEFAULT 0,
  `PowerLiquor` bigint(18) NOT NULL,
  `PowerLiquorPrize` bigint(18) DEFAULT NULL,
  `Prize_Reseved` bigint(255) DEFAULT 0,
  `DragonBallItemPromoter` bigint(255) DEFAULT 60,
  `StoneItemPromoter` bigint(255) DEFAULT 100,
  `PermanentStoneItemPromoter` bigint(255) DEFAULT 60,
  `ChiPackItemPromoter` bigint(255) DEFAULT 200,
  `ToughDrillItemPromoter` bigint(255) DEFAULT 40,
  `StarDrillItemPromoter` bigint(255) DEFAULT 10,
  `CPExchange` bigint(255) DEFAULT 34000,
  PRIMARY KEY (`UID`,`claimed`) USING BTREE,
  UNIQUE KEY `myIndex` (`UID`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `news`
-- ----------------------------
DROP TABLE IF EXISTS `news`;
CREATE TABLE `news` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL DEFAULT '',
  `article` longtext NOT NULL,
  `writeruid` int(255) NOT NULL,
  `time` int(255) NOT NULL,
  `Year` int(5) NOT NULL,
  `month` varchar(255) NOT NULL,
  `day` varchar(255) NOT NULL,
  `likes` int(255) DEFAULT 0,
  `iconstyle` varchar(255) DEFAULT 'fa fa-envelope bg-blue',
  `Image` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `nobility`
-- ----------------------------
DROP TABLE IF EXISTS `nobility`;
CREATE TABLE `nobility` (
  `EntityUID` bigint(255) unsigned NOT NULL DEFAULT 0,
  `EntityName` varchar(16) NOT NULL DEFAULT '',
  `Donation` bigint(255) unsigned DEFAULT 0
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `online`
-- ----------------------------
DROP TABLE IF EXISTS `online`;
CREATE TABLE `online` (
  `Name` varchar(255) NOT NULL,
  `OnlineCount` int(11) DEFAULT 0,
  `PlayersOnline` int(11) DEFAULT 0,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `panelattack`
-- ----------------------------
DROP TABLE IF EXISTS `panelattack`;
CREATE TABLE `panelattack` (
  `SkilUID` bigint(20) NOT NULL,
  `Damge` double NOT NULL DEFAULT 0,
  PRIMARY KEY (`SkilUID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Records of panelattack
-- ----------------------------
INSERT INTO `panelattack` VALUES ('1000', '1');
INSERT INTO `panelattack` VALUES ('1001', '1.11');
INSERT INTO `panelattack` VALUES ('1002', '1.11');
INSERT INTO `panelattack` VALUES ('1010', '1.11');
INSERT INTO `panelattack` VALUES ('1045', '0.7');
INSERT INTO `panelattack` VALUES ('1046', '1');
INSERT INTO `panelattack` VALUES ('1115', '1.23');
INSERT INTO `panelattack` VALUES ('1120', '1.11');
INSERT INTO `panelattack` VALUES ('1125', '1.11');
INSERT INTO `panelattack` VALUES ('1150', '1.11');
INSERT INTO `panelattack` VALUES ('1160', '1.11');
INSERT INTO `panelattack` VALUES ('1165', '1.11');
INSERT INTO `panelattack` VALUES ('1180', '1.11');
INSERT INTO `panelattack` VALUES ('1260', '1.11');
INSERT INTO `panelattack` VALUES ('1290', '1.11');
INSERT INTO `panelattack` VALUES ('3090', '1.11');
INSERT INTO `panelattack` VALUES ('5001', '1.11');
INSERT INTO `panelattack` VALUES ('5010', '1.11');
INSERT INTO `panelattack` VALUES ('5030', '1');
INSERT INTO `panelattack` VALUES ('5040', '1.11');
INSERT INTO `panelattack` VALUES ('6000', '1.11');
INSERT INTO `panelattack` VALUES ('6003', '1.11');
INSERT INTO `panelattack` VALUES ('6010', '1.11');
INSERT INTO `panelattack` VALUES ('7000', '1.1');
INSERT INTO `panelattack` VALUES ('7020', '1');
INSERT INTO `panelattack` VALUES ('7030', '1.11');
INSERT INTO `panelattack` VALUES ('8000', '1.11');
INSERT INTO `panelattack` VALUES ('8001', '1.11');
INSERT INTO `panelattack` VALUES ('10309', '1.11');
INSERT INTO `panelattack` VALUES ('10310', '1.11');
INSERT INTO `panelattack` VALUES ('10315', '1');
INSERT INTO `panelattack` VALUES ('10381', '1.11');
INSERT INTO `panelattack` VALUES ('10390', '1.11');
INSERT INTO `panelattack` VALUES ('10415', '1.11');
INSERT INTO `panelattack` VALUES ('10490', '1.11');
INSERT INTO `panelattack` VALUES ('11000', '1.11');
INSERT INTO `panelattack` VALUES ('11005', '1.11');
INSERT INTO `panelattack` VALUES ('11040', '1.11');
INSERT INTO `panelattack` VALUES ('11050', '1.11');
INSERT INTO `panelattack` VALUES ('11070', '1.11');
INSERT INTO `panelattack` VALUES ('11100', '1.11');
INSERT INTO `panelattack` VALUES ('11110', '1.11');
INSERT INTO `panelattack` VALUES ('11140', '0.2');
INSERT INTO `panelattack` VALUES ('11170', '1.11');
INSERT INTO `panelattack` VALUES ('11190', '1.11');
INSERT INTO `panelattack` VALUES ('11230', '1.11');
INSERT INTO `panelattack` VALUES ('11600', '1.11');
INSERT INTO `panelattack` VALUES ('11610', '1.11');
INSERT INTO `panelattack` VALUES ('11650', '0.7');
INSERT INTO `panelattack` VALUES ('11660', '1.11');
INSERT INTO `panelattack` VALUES ('11980', '1.11');
INSERT INTO `panelattack` VALUES ('11990', '1.11');
INSERT INTO `panelattack` VALUES ('12070', '1.11');
INSERT INTO `panelattack` VALUES ('12080', '1.11');
INSERT INTO `panelattack` VALUES ('12110', '1.11');
INSERT INTO `panelattack` VALUES ('12120', '1.11');
INSERT INTO `panelattack` VALUES ('12130', '1.11');
INSERT INTO `panelattack` VALUES ('12140', '1.11');
INSERT INTO `panelattack` VALUES ('12160', '1.11');
INSERT INTO `panelattack` VALUES ('12170', '1.11');
INSERT INTO `panelattack` VALUES ('12240', '1.11');
INSERT INTO `panelattack` VALUES ('12290', '1.11');
INSERT INTO `panelattack` VALUES ('12300', '1.11');
INSERT INTO `panelattack` VALUES ('12320', '1.12');
INSERT INTO `panelattack` VALUES ('12330', '1.11');
INSERT INTO `panelattack` VALUES ('12340', '1.11');
INSERT INTO `panelattack` VALUES ('12350', '1.11');
INSERT INTO `panelattack` VALUES ('12550', '1.11');
INSERT INTO `panelattack` VALUES ('12570', '1.11');
INSERT INTO `panelattack` VALUES ('12600', '1.11');
INSERT INTO `panelattack` VALUES ('12670', '1.11');
INSERT INTO `panelattack` VALUES ('12690', '1.11');
INSERT INTO `panelattack` VALUES ('12700', '1.11');
INSERT INTO `panelattack` VALUES ('12840', '1.11');
INSERT INTO `panelattack` VALUES ('12850', '1.12');
INSERT INTO `panelattack` VALUES ('12860', '1.11');
INSERT INTO `panelattack` VALUES ('12890', '1.11');
INSERT INTO `panelattack` VALUES ('12930', '1.11');
INSERT INTO `panelattack` VALUES ('12940', '1.11');
INSERT INTO `panelattack` VALUES ('12980', '1.11');
INSERT INTO `panelattack` VALUES ('13190', '1.11');

-- ----------------------------
-- Table structure for `partners`
-- ----------------------------
DROP TABLE IF EXISTS `partners`;
CREATE TABLE `partners` (
  `partners_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `image` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`partners_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `payment_logs`
-- ----------------------------
DROP TABLE IF EXISTS `payment_logs`;
CREATE TABLE `payment_logs` (
  `txn_id` varchar(32) NOT NULL,
  `gateway` varchar(64) NOT NULL,
  `amount` decimal(4,2) NOT NULL,
  `item` varchar(64) NOT NULL,
  `email` varchar(64) NOT NULL,
  `account` varchar(34) NOT NULL,
  `coins` smallint(5) NOT NULL,
  `paid` tinyint(1) NOT NULL,
  `timestamp` datetime NOT NULL,
  PRIMARY KEY (`txn_id`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `payments`
-- ----------------------------
DROP TABLE IF EXISTS `payments`;
CREATE TABLE `payments` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `product_id` int(11) NOT NULL,
  `txn_id` text NOT NULL,
  `IDPayment` varchar(255) NOT NULL,
  `payment_gross` double(8,2) NOT NULL,
  `currency_code` varchar(255) NOT NULL,
  `payer_id` varchar(255) NOT NULL,
  `payer_name` varchar(255) NOT NULL,
  `payer_email` varchar(255) NOT NULL,
  `payer_country` varchar(255) NOT NULL,
  `payment_status` varchar(255) NOT NULL,
  `user` varchar(255) NOT NULL,
  `token` varchar(255) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  `points` int(11) DEFAULT NULL,
  `clamed` int(11) DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `player`
-- ----------------------------
DROP TABLE IF EXISTS `player`;
CREATE TABLE `player` (
  `cps` varchar(255) DEFAULT NULL,
  `vip` varchar(255) DEFAULT NULL,
  `uid` bigint(20) unsigned NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Face` bigint(20) DEFAULT NULL,
  `Level` bigint(20) DEFAULT NULL,
  `PkPoints` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `playersonline`
-- ----------------------------
DROP TABLE IF EXISTS `playersonline`;
CREATE TABLE `playersonline` (
  `Online` bigint(32) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`Online`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=FIXED;

-- ----------------------------
-- Table structure for `points`
-- ----------------------------
DROP TABLE IF EXISTS `points`;
CREATE TABLE `points` (
  `user` varchar(16) NOT NULL,
  `bonus` int(11) DEFAULT 0,
  PRIMARY KEY (`user`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=cp1256 COLLATE=cp1256_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `post`
-- ----------------------------
DROP TABLE IF EXISTS `post`;
CREATE TABLE `post` (
  `ID` varchar(255) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Image` varchar(255) DEFAULT NULL,
  `Title` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `posts`
-- ----------------------------
DROP TABLE IF EXISTS `posts`;
CREATE TABLE `posts` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(30) NOT NULL,
  `post` text NOT NULL,
  `user` varchar(30) NOT NULL,
  `date` varchar(30) NOT NULL,
  `Image` varchar(30) NOT NULL DEFAULT 'default.png',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `rankings`
-- ----------------------------
DROP TABLE IF EXISTS `rankings`;
CREATE TABLE `rankings` (
  `UID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `Donation` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- ----------------------------
-- Table structure for `redeem`
-- ----------------------------
DROP TABLE IF EXISTS `redeem`;
CREATE TABLE `redeem` (
  `UID` bigint(255) NOT NULL,
  `ServerID` bigint(16) unsigned DEFAULT 1,
  `done` varchar(22) NOT NULL DEFAULT '',
  PRIMARY KEY (`UID`) USING BTREE,
  UNIQUE KEY `qwewas` (`UID`) USING BTREE,
  KEY `eeee` (`ServerID`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `servers`
-- ----------------------------
DROP TABLE IF EXISTS `servers`;
CREATE TABLE `servers` (
  `Name` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `IP` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Port` int(16) unsigned DEFAULT NULL,
  `TransferKey` varchar(64) CHARACTER SET latin1 COLLATE latin1_general_cs NOT NULL,
  `TransferSalt` varchar(64) CHARACTER SET latin1 COLLATE latin1_general_cs NOT NULL,
  `PlayersOnline` int(11) DEFAULT 0,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `shop`
-- ----------------------------
DROP TABLE IF EXISTS `shop`;
CREATE TABLE `shop` (
  `name_item` varchar(255) NOT NULL,
  `value_item` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `desc_item` text DEFAULT NULL,
  `img_item` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`name_item`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `slider`
-- ----------------------------
DROP TABLE IF EXISTS `slider`;
CREATE TABLE `slider` (
  `slider_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(100) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `image` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`slider_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=28 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `social`
-- ----------------------------
DROP TABLE IF EXISTS `social`;
CREATE TABLE `social` (
  `social_id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `face` varchar(100) DEFAULT NULL,
  `twitter` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`social_id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `spellseditor`
-- ----------------------------
DROP TABLE IF EXISTS `spellseditor`;
CREATE TABLE `spellseditor` (
  `ID` tinyint(4) DEFAULT NULL,
  `IncreaseDMG` tinyint(4) DEFAULT NULL,
  `DecreaseDMG` tinyint(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `store`
-- ----------------------------
DROP TABLE IF EXISTS `store`;
CREATE TABLE `store` (
  `ID` int(255) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `Price_LE` int(6) DEFAULT NULL,
  `Price_USD` int(6) DEFAULT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `stores`
-- ----------------------------
DROP TABLE IF EXISTS `stores`;
CREATE TABLE `stores` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `item` varchar(255) NOT NULL,
  `img` text NOT NULL,
  `price` double(8,2) NOT NULL,
  `Quantity` double(8,2) NOT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `tickets`
-- ----------------------------
DROP TABLE IF EXISTS `tickets`;
CREATE TABLE `tickets` (
  `id` int(255) NOT NULL AUTO_INCREMENT,
  `UserUID` int(255) NOT NULL,
  `category` varchar(255) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `problem` longtext NOT NULL,
  `time` datetime NOT NULL,
  `Status` int(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `tickets_replys`
-- ----------------------------
DROP TABLE IF EXISTS `tickets_replys`;
CREATE TABLE `tickets_replys` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `ticketid` int(5) NOT NULL,
  `reply` text NOT NULL,
  `user` varchar(30) NOT NULL,
  `date` varchar(30) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `vodacard`
-- ----------------------------
DROP TABLE IF EXISTS `vodacard`;
CREATE TABLE `vodacard` (
  `Email` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `fbID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `cardNum` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `hmCash` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `phNum` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `vodacash`
-- ----------------------------
DROP TABLE IF EXISTS `vodacash`;
CREATE TABLE `vodacash` (
  `Email` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `fbID` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `opNum` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `hmCash` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `phNum` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `vote`
-- ----------------------------
DROP TABLE IF EXISTS `vote`;
CREATE TABLE `vote` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user` varchar(16) DEFAULT NULL,
  `ip` varchar(100) DEFAULT NULL,
  `time` int(20) DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=4163 DEFAULT CHARSET=cp1256 COLLATE=cp1256_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `votes`
-- ----------------------------
DROP TABLE IF EXISTS `votes`;
CREATE TABLE `votes` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `user` varchar(30) NOT NULL,
  `ip_address` varchar(150) NOT NULL,
  `time` varchar(50) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `vtm_news`
-- ----------------------------
DROP TABLE IF EXISTS `vtm_news`;
CREATE TABLE `vtm_news` (
  `id_news` int(11) NOT NULL AUTO_INCREMENT,
  `title_news` varchar(255) DEFAULT NULL,
  `subject_news` longtext DEFAULT NULL,
  `img_news` varchar(255) DEFAULT NULL,
  `time_news` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp(),
  PRIMARY KEY (`id_news`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Table structure for `vtm_payments`
-- ----------------------------
DROP TABLE IF EXISTS `vtm_payments`;
CREATE TABLE `vtm_payments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `item_id` text NOT NULL,
  `cliam` tinyint(1) DEFAULT 0,
  `date` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

