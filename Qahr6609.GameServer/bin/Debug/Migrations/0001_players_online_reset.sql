-- 0001_players_online_reset
-- players.Online is kept up to date by the server every ~15 s (PlayerStorage.OnlineSync); start every existing
-- row from 0 so no row stays "online" after an old crash. Runs once (recorded in schema_migrations).
UPDATE `players` SET `Online` = 0 WHERE `Online` <> 0;
