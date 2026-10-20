# Security phase 1: deploy notes

Short checklist for putting the hardened AccountServer and game server live, and for rolling each
feature back if it gets in the way.

## 1. Before you deploy (in this order)

1. **Back up** the MySQL database and the whole `Database` folder (DbLocation).
2. **Create the login token table.** Run `docs/security/login_tokens.sql` against the database that
   holds `accounts`. If you skip this, every login is refused. Both servers print a large red
   `login_tokens is NOT usable` banner at startup when the table is missing.
3. **Set account roles.** The in-game GM chat password is gone. GM rights now come only from
   `accounts.State`:
   ```sql
   UPDATE accounts SET State = 255 WHERE Username = 'owner_login';   -- owner: project manager + GM
   UPDATE accounts SET State = 3   WHERE Username IN ('gm1', 'gm2'); -- GMs
   ```
   Anything else (2 = player, 1 = banned, 100 = not activated) gets no GM rights. Banned and
   not-activated accounts are refused by the AccountServer.

   The `[GameMasters]` section in `shell.ini` (added with the Ramadan work, `Database/GameMasters.cs`)
   still works on top of this: a character UID listed in `UIDs=` gets GM at login, but only from the
   IPs in `AllowedIPs=` (default: 127.0.0.1 and the server's own IP). Keep `AllowedIPs` at `default`
   or a short list of fixed IPs; do not set it to `any`. Use `accounts.State` for GMs who play from home.
4. Add the `shell.ini` keys from section 2 if you need anything other than the defaults. Missing
   keys use the defaults.
5. Start the AccountServer and the game server, check both consoles for red banners, then log in
   once with a normal account and once with the owner account.

## 2. Configuration

### Game server `shell.ini`, section `[Security]`

| Key | Default | Meaning |
|---|---|---|
| `VerifyLoginTokens` | `1` | The game server loads a character only with a fresh, single-use token from the AccountServer. `0` turns the check off. |
| `VerifyTokenIp` | `1` | The token must come from the same IP that logged in. Loopback (127.0.0.1, ::1) and private LAN addresses (10/8, 172.16/12, 192.168/16, 169.254/16, fc00::/7, fe80::/10) count as the same address. `0` ignores the IP, which is needed for hairpin NAT or when the AccountServer sees a public IP but the game server sees a LAN IP. |
| `LenientLegacyLoad` | `1` | A player file that fails validation and has no valid `.bak` is still loaded with the old tolerant reader. A copy is kept as `<file>.corrupt-<time>` and a `legacy-load` line goes to the audit log. `0` refuses the login: the player sees a dialog and nothing is overwritten. |
| `StrictNpcOptions` | `1` | NPC dialog option checks. `0` = off, `1` = audit only (first rollout: watch `npc-option` lines in the audit log), `2` = enforce. Move to `2` once the log is clean. |
| `MaxMoneyAmount` | `10000000000` | Maximum silver for a single operation (trade, warehouse, vendor price...). |
| `MaxWarehouseMoney` | `50000000000` | Maximum silver held in a warehouse. |
| `MaxCpsAmount` | `1000000000` | Maximum CPs for a single operation. |
| `MaxDonation` | `10000000000` | Maximum single donation. |
| `MaxJumpDistance` | `18` | Maximum distance for one jump. Longer jumps are rejected. |
| `MinJumpIntervalMs` | `300` | Minimum time between jumps. |
| `FastJumpViolationLimit` | `3` | Fast jumps tolerated within the window before the player is pulled back. |
| `FastJumpWindowMs` | `5000` | Window for counting fast jumps. |
| `VipTeleportMinLevel` | `1` | Minimum active VIP level (own or shared) for VIP teleports. |
| `VipTeamTeleportSameMap` | `1` | `1` = VIP team teleport only reaches members on the same map. |
| `NpcDialogTimeoutSec` | `300` | How long an opened NPC dialog stays valid for option replies. |

### Game server `shell.ini`, section `[InterServer]`

| Key | Default | Meaning |
|---|---|---|
| `PipeEnabled` | `0` | Opens the inter-server pipe listener on a normal (non inter-) server. The inter-server always listens. |
| `Secret` | *(empty)* | Shared secret for the pipe link. **With no secret, the listener does not start.** Use the same value on every server in the group. |
| `AllowedIPs` | *(empty)* | Comma-separated list of peers allowed to connect. When empty, only 127.0.0.1 and the configured group/inter servers are accepted. |

### AccountServer `AuthConfig.ini`, section `[Security]`

| Key | Default | Meaning |
|---|---|---|
| `MaxFailuresPerIp` | `5` | Failed logins from one IP, within the window, before that IP is locked. |
| `MaxFailuresPerUser` | `3` | Failed logins for one username **from one IP** before that username is locked for that IP only. Other IPs, including the real owner's, can still log in. |
| `FailureWindowMinutes` | `15` | Sliding window for counting failures. |
| `LockoutMinutes` | `5` | How long a lock lasts. |
| `MaxAttemptsPerConnection` | `3` | Authentication packets accepted on one connection. |

Login tokens live for 300 s, are single-use and are bound to the IP. If storing the token fails, the
AccountServer replies "invalid info" and logs `login-token-error`. It does not hand the client off.

## 3. Where to look when something goes wrong

- **Audit log:** `Security_Logs\yyyyMMdd.log` in the working directory of each server, with one
  tab-separated line per event: time, category, UID, name, details. Useful categories:
  `login-rejected` (token problems, with the reason), `login-load-failed`, `load-fallback` (loaded
  from `.bak`), `legacy-load`, `load-error`, `save-error`, `item-uid-implausible`, `gm-login`,
  `ip-lockout`, `user-ip-lockout`, `login-failed`, `login-token-error`, `interserver`, `security-config`.
- **Damaged player files** are kept next to the original as `<file>.corrupt-yyyyMMddHHmmss`.
  Every save leaves the previous version as `<file>.bak`, and a save in progress is written to
  `<file>.tmp`. Guild, clan and rank loaders ignore all of these files.

## 4. Rolling back a feature

| Problem | Quick fix (no rebuild) |
|---|---|
| Nobody can log in, token errors | Apply `login_tokens.sql`, or set `[Security] VerifyLoginTokens=0` and restart the game server. |
| Players behind NAT, on the host or on a LAN are rejected with "ip mismatch" | Set `[Security] VerifyTokenIp=0`. |
| A player is refused because of damaged files | Keep `LenientLegacyLoad=1` (default), or restore that player's `.bak` / `.corrupt-*` file by hand. |
| Valid NPC options are refused | Set `StrictNpcOptions=1` (audit only) and send the `Security_Logs` lines to the developer. |
| Legitimate big trades or donations are refused | Raise `MaxMoneyAmount`, `MaxCpsAmount`, `MaxDonation` or `MaxWarehouseMoney`. |
| Valid jumps are rejected | Raise `MaxJumpDistance` or lower `MinJumpIntervalMs`. |
| Inter-server link does not connect | Set the same `Secret` on every server, add peer IPs to `AllowedIPs`, and set `PipeEnabled=1` where needed. |
| GM lost rights | Set `accounts.State` to 3 or 255 (section 1). There is no chat-password fallback. |
| Real users get locked out at the login screen | Raise `MaxFailuresPerIp` / `MaxFailuresPerUser` or lower `LockoutMinutes` in `AuthConfig.ini`. |

A full rollback means redeploying the previous binaries. The data formats have not changed, but
**before you start an old build, delete `*.bak`, `*.tmp` and `*.corrupt-*` from `Database\Guilds`,
`Database\Clans` and `Database\PlayersItems`.** The old guild, clan and rank loaders read every
file in those folders and would load the backups as duplicate guilds or clans. The old build ignores
these files in the other folders.
