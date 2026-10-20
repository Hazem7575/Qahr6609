<#
  Creates the folder of a new game server (or the Realm) from an existing one, without player data, and adds it
  to HazemConfig.ini with Published=0 Enabled=0 (docs\MULTI_SERVER.md). Nothing is started.

    new_server.ps1 -Id 2 -Name Sphinx -Port 5818 [-PipePort 9917] [-From Server1] [-Dest _servers\Sphinx] [-Config HazemConfig.ini]
    new_server.ps1 -Realm [-From Server1]          (folder of the [Realm] entry, IsInterServer=1)

  Steps: checks (id / name / ports free, destination empty) -> copy exe + static data (robocopy) without characters,
  world state, logs and backups -> empty player folders -> new [ServerN] section in the master file (item UID range
  (Id-1)*100M+1 .. Id*100M-1) -> configure.ps1 -Only <section> writes its shell.ini (ports, ServerID, ItemUID range,
  MySQL, LoginKey ...) and client_config.ini.
  client_config.ini must be identical in every server folder: run configure.bat (all) before publishing the server.
#>
param(
    [int]$Id = 0,
    [string]$Name = '',
    [int]$Port = 0,
    [int]$PipePort = 0,
    [string]$From = 'Server1',
    [string]$Dest = '',
    [string]$Config = '',
    [switch]$Realm
)
$ErrorActionPreference = 'Stop'
$RepoRoot = Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
if (-not $Config) { $Config = Join-Path $RepoRoot 'Qahr6609.Shared\HazemConfig.ini' }
$Config = (Resolve-Path $Config).Path
$Root = Split-Path $Config -Parent

function Read-Master([string]$file) {
    $sections = [ordered]@{}; $cur = $null
    foreach ($raw in [IO.File]::ReadAllLines($file)) {
        $line = $raw; $i = $line.IndexOf(';'); if ($i -ge 0) { $line = $line.Substring(0, $i) }; $line = $line.Trim()
        if (-not $line) { continue }
        if ($line.StartsWith('[')) { $cur = $line.Trim('[', ']').Trim(); $sections[$cur] = [ordered]@{ Section = $cur }; continue }
        if ($null -eq $cur) { continue }
        $eq = $line.IndexOf('='); if ($eq -le 0) { continue }
        $sections[$cur][$line.Substring(0, $eq).Trim()] = $line.Substring($eq + 1).Trim()
    }
    return $sections
}
function Abs([string]$p) { if ([IO.Path]::IsPathRooted($p)) { return [IO.Path]::GetFullPath($p) }; return [IO.Path]::GetFullPath((Join-Path $Root $p)) }
function PortUsed([int]$p) {
    if ($p -le 0) { return $false }
    return @(Get-NetTCPConnection -LocalPort $p -State Listen -ErrorAction SilentlyContinue).Count -gt 0
}

$M = Read-Master $Config
if (-not $M.Contains($From)) { throw "source entry [$From] not in $Config" }
$src = Abs $M[$From].Folder
if (-not (Test-Path (Join-Path $src 'GameServer.exe'))) { throw "source folder $src has no GameServer.exe" }

if ($Realm) {
    if (-not $M.Contains('Realm')) { throw "no [Realm] section in $Config" }
    $section = 'Realm'; $entry = $M['Realm']
    $Name = $entry.Name; $Id = [int]$entry.ServerID
    if (-not $Dest) { $Dest = $entry.Folder }
} else {
    if ($Id -lt 1 -or $Id -gt 19) { throw '-Id 1..19 is required (item UID ranges: (Id-1)*100M+1 .. Id*100M-1)' }
    if (-not $Name -or $Name -notmatch '^[A-Za-z0-9_]{2,15}$') { throw '-Name (2-15 letters/digits, no spaces) is required' }
    if ($Port -le 0) { throw '-Port <game port> is required' }
    if ($PipePort -le 0) { $PipePort = 9915 + $Id }
    $section = "Server$Id"
    foreach ($k in $M.Keys) {
        $s = $M[$k]
        if ($k -eq $section) { throw "[$section] already exists in $Config" }
        if ($s.ServerID -and [int]$s.ServerID -eq $Id) { throw "ServerID $Id is already used by [$k]" }
        if ($s.Name -and $s.Name -ieq $Name) { throw "name $Name is already used by [$k]" }
        if ($s.GamePort -and [int]$s.GamePort -eq $Port) { throw "game port $Port is already used by [$k]" }
        if ($s.PipePort -and [int]$s.PipePort -eq $PipePort) { throw "pipe port $PipePort is already used by [$k]" }
    }
    if (PortUsed $Port) { throw "port $Port is already listening on this PC" }
    if (-not $Dest) { $Dest = "_servers\$Name" }
}
$dst = Abs $Dest
if ((Test-Path $dst) -and @(Get-ChildItem $dst -Force).Count -gt 0) { throw "destination $dst exists and is not empty" }

# ------------------------------------------------------------------ copy without player / world data
$playerDirs = 'Users', 'PlayersItems', 'PlayersSpells', 'PlayersProfs', 'Quests', 'PlayersMail', 'Houses', 'Guilds', 'Clans', 'Unions',
              'AABackUP', 'BackUp', 'Agates', 'Retreate', 'Halo_Flag', 'ClanWar'
$xd = @('Exceptions', 'Co_Loggs', 'Loader_Logs', 'Security_Logs', 'app.publish') + ($playerDirs | ForEach-Object { Join-Path $src "ServerDataBase\$_" })
$xd += @(Join-Path $src 'ServerDataBase\RamadanEvent\Players'), (Join-Path $src 'ServerDataBase\RamadanEvent\Logs')
Get-ChildItem (Join-Path $src 'ServerDataBase\Modules') -Directory -ErrorAction SilentlyContinue | ForEach-Object {
    $xd += (Join-Path $_.FullName 'players'), (Join-Path $_.FullName 'logs')
}
Write-Host "Copying $src -> $dst (static data only)..."
$rcArgs = @($src, $dst, '/E', '/NFL', '/NDL', '/NJH', '/NP', '/R:1', '/W:1', '/XF', '*.bak', '*.pid', 'shell.ini.bak', '*.orig', '/XD') + $xd
& robocopy @rcArgs | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE)" }
foreach ($d in $playerDirs) { New-Item -ItemType Directory -Force (Join-Path $dst "ServerDataBase\$d") | Out-Null }

# world state saved by the source server (every file the save writes keeps a .bak next to it) + a few without one
$db = Join-Path $src 'ServerDataBase'
$state = @(Get-ChildItem $db -File -Filter '*.bak' | ForEach-Object { $_.Name.Substring(0, $_.Name.Length - 4) })
$state += 'Comatetor.ini', 'CouplesPK.ini', 'GuildWarInfo.ini', 'SuperGuildWarInfo.ini', 'KingdomWar.ini', 'PerfectionRanking.bin', 'StaticStatue.txt', 'itemtype.txt.bak'
$removed = 0
foreach ($f in ($state | Sort-Object -Unique)) {
    if ($f -ieq 'itemtype.txt') { continue }   # static data that happens to have an old .bak
    $p = Join-Path $dst "ServerDataBase\$f"
    if (Test-Path $p) { Remove-Item $p -Force; $removed++ }
}
Write-Host "Removed $removed world-state file(s) of the source server (rankings, wars, bans, containers...)."

# counters / winners of the source server must not carry over (shell.ini keys outside the master's)
$shell = Join-Path $dst 'shell.ini'
if (Test-Path $shell) {
    $t = [IO.File]::ReadAllText($shell, [Text.Encoding]::GetEncoding(28591))
    $t = [Text.RegularExpressions.Regex]::Replace($t, '(?im)^(ItemUID|PkWarWinner|MonthlyPkWarWinner)\s*=.*?(\r?)$', '$1=0$2')
    [IO.File]::WriteAllText($shell, $t, [Text.Encoding]::GetEncoding(28591))
}

# ------------------------------------------------------------------ master entry
if (-not $Realm) {
    $min = ($Id - 1) * 100000000 + 1; $max = $Id * 100000000 - 1
    $relDest = if ($dst.StartsWith($Root, [StringComparison]::OrdinalIgnoreCase)) { $dst.Substring($Root.Length).TrimStart('\') } else { $dst }
    $hostIp = if ($M.Contains('Global') -and $M['Global'].PublicIP) { $M['Global'].PublicIP } else { '127.0.0.1' }
    $block = @"

[$section]
Name=$Name                      ; shell.ini ServerName = client_config.ini name = name shown in the client list
LoginName=$Name                 ; name written into the client's Guard.dat (what the client sends at login)
ServerID=$Id                    ; unique id
Folder=$relDest                 ; working folder of the process
Exe=GameServer.exe
Host=$hostIp                    ; IP the AccountServer sends players to
GamePort=$Port                  ; game port
PipePort=$PipePort              ; pipe port (client_config.ini)
Published=0                     ; 0 = hidden (staff only) until it is ready
Enabled=0                       ; 0 = the control panel does not start it with "start all"
ItemUIDMin=$min
ItemUIDMax=$max
"@
    $bak = Join-Path $RepoRoot ("_backup\new_server-" + (Get-Date -Format 'yyyyMMdd-HHmmss'))
    New-Item -ItemType Directory -Force $bak | Out-Null
    Copy-Item $Config $bak -Force
    [IO.File]::AppendAllText($Config, ($block -replace "`r?`n", "`r`n"))
    Write-Host "[$section] added to $Config (Published=0 Enabled=0); old master backed up in $bak"
}

& (Join-Path $PSScriptRoot 'configure.ps1') -Config $Config -Only $section
Write-Host ""
Write-Host "Done: $dst"
Write-Host "Next: configure.bat (all folders get the new client_config.ini row), then cluster.bat start $Name"
