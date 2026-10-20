-- 0002_player_blobs
-- The player .bin files (items / mail / profs / spells / quests / house), byte for byte, as in Qahr.
-- `store` = the folder of the file (PlayersItems, PlayersMail, ...). Written by PlayerBlobStorage.
-- `file_time` = write time (UTC ticks) of the file the bytes came from: a DB read uses the blob only while it matches.
CREATE TABLE IF NOT EXISTS `player_blobs` (
  `uid` INT UNSIGNED NOT NULL,
  `store` VARCHAR(32) NOT NULL,
  `data` LONGBLOB NOT NULL,
  `file_time` BIGINT NOT NULL DEFAULT 0,
  `updated_at` DATETIME NOT NULL,
  PRIMARY KEY (`uid`, `store`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
