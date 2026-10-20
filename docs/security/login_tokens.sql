-- One-time login handoff tokens (AccountServer -> game server).
-- The AccountServer REPLACEs a row after a successful password check; the game server
-- accepts the login only if the row exists for that EntityID, Used = 0, ExpiresAt is in
-- the future (tokens live 300 s) and IP matches the connecting socket's IP, then sets Used = 1
-- atomically. IP matching (game server shell.ini [Security] VerifyTokenIp, default 1): same IP, or
-- both loopback / private LAN addresses; VerifyTokenIp=0 ignores the IP column.
-- Both servers use the database clock (NOW()), so their local clocks do not matter.
-- Apply to the same database that contains the `accounts` table.
CREATE TABLE IF NOT EXISTS `login_tokens` (
  `EntityID`  BIGINT      NOT NULL,
  `IP`        VARCHAR(64) NOT NULL DEFAULT '',
  `ExpiresAt` DATETIME    NOT NULL,
  `Used`      TINYINT(1)  NOT NULL DEFAULT 0,
  PRIMARY KEY (`EntityID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Optional housekeeping (rows are overwritten on each login, so the table stays small):
-- DELETE FROM `login_tokens` WHERE `ExpiresAt` < NOW() - INTERVAL 1 DAY;
