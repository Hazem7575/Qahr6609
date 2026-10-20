<#
  Hazem configure: writes every derived settings file from the master HazemConfig.ini (docs\MULTI_SERVER.md).

    configure.bat                 write all derived files (old ones backed up to _backup\configure-<time>\)
    configure.bat -Check          only show what would change (secrets masked), write nothing
    configure.bat -Only Server2   only the files of these entries (comma list of sections; Auth, Launcher, Guard too)
    configure.bat -GuardDat       also write the encrypted client list <ClientPatchFolder>\Guard.dat
    configure.bat -Config <file>  another master file (e.g. a sandbox)

  Files (only the keys owned by the master are changed, every other key / comment stays as it is):
    <server folder>\shell.ini                [ServerInfo] AddresIP Auth_Port Game_Port ServerName ServerID Db*  [Database] ItemUIDMin/Max
                                             [InterServer] AddresIP Port IsInterServer Secret Bind RealmName  [Security] LoginKey TransferKey TransferSalt
    <server folder>\ServerDataBase\client_config.ini   one row per [ServerN]/[Realm] (identical in every folder)
    [Auth] folder\AuthConfig.ini             [ServerInfo] Auth_Port ServerCount ServerN (fallback list)  [MySql] Host Port User Password Database
    <LauncherFolder>\launcher.ini, <ClientPatchFolder>\launcher.ini   [Launcher] UpdateUrl ServerIP LoginPort
    <PatchServerFolder>\PatchServer.ini      [Server] Port  [Servers] LoginHost LoginPort
    [Global] GuardTxt                        plain client server list (Guard.dat layout); Guard.dat itself only with -GuardDat
#>
param(
    [string]$Config = '',
    [switch]$Check,
    [switch]$GuardDat,
    [string]$Only = ''
)
$ErrorActionPreference = 'Stop'
$RepoRoot = Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
if (-not $Config) { $Config = Join-Path $RepoRoot 'Qahr6609.Shared\HazemConfig.ini' }
if (-not (Test-Path $Config)) { throw "master file not found: $Config (copy HazemConfig.example.ini and fill it)" }
$Config = (Resolve-Path $Config).Path
$Root = Split-Path $Config -Parent
$Latin1 = [Text.Encoding]::GetEncoding(28591)   # byte-exact round trip for ANSI / UTF-8 files
$Stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$BackupDir = Join-Path $RepoRoot "_backup\configure-$Stamp"
$OnlyList = @($Only.Split(',') | ForEach-Object { $_.Trim() } | Where-Object { $_ })
$SecretKey = '(?i)pass|secret|key|salt'

# ------------------------------------------------------------------ master
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
$M = Read-Master $Config
$G = if ($M.Contains('Global')) { $M['Global'] } else { @{} }
function CfgVal([string]$k, [string]$def = '') { if ($G.Contains($k) -and $G[$k] -ne '') { return $G[$k] } return $def }
function FolderOf($s) { if (-not $s.Folder) { return $null }; if ([IO.Path]::IsPathRooted($s.Folder)) { return $s.Folder }; return [IO.Path]::GetFullPath((Join-Path $Root $s.Folder)) }
function Want([string]$name) { return $OnlyList.Count -eq 0 -or ($OnlyList -contains $name) }

$PublicIP = CfgVal 'PublicIP'
$AuthPort = CfgVal 'AuthPort' '9961'
$AuthHost = CfgVal 'AuthHost' $PublicIP
$Games = @($M.Keys | Where-Object { $_ -like 'Server*' } | ForEach-Object { $M[$_] } | Sort-Object { [int]$_.ServerID })
$Realm = if ($M.Contains('Realm')) { $M['Realm'] } else { $null }
$Published = @($Games | Where-Object { $_.Published -eq '1' })

# sanity checks of the master itself
$ids = @{}; $ranges = @()
foreach ($e in @($Games) + @($Realm | Where-Object { $_ })) {
    if (-not $e.ServerID) { throw "[$($e.Section)] has no ServerID" }
    if ($ids.ContainsKey($e.ServerID)) { throw "ServerID $($e.ServerID) is used by [$($ids[$e.ServerID])] and [$($e.Section)]" }
    $ids[$e.ServerID] = $e.Section
    if ($e.ItemUIDMin) {
        $lo = [int64]$e.ItemUIDMin; $hi = [int64]$e.ItemUIDMax
        if ($lo -lt 1 -or $hi -le $lo -or $hi -gt 2000000000) { throw "[$($e.Section)] ItemUIDMin/Max $lo-$hi is not a valid range" }
        foreach ($r in $ranges) { if ($lo -le $r[2] -and $r[1] -le $hi) { throw "item UID range of [$($e.Section)] overlaps [$($r[0])]" } }
        $ranges += , @($e.Section, $lo, $hi)
    }
}

# ------------------------------------------------------------------ ini editing (keeps everything not owned by the master)
$script:Changes = 0
function Edit-Ini([string]$text, $sets, [string]$label) {
    $lines = [Collections.Generic.List[string]]($text -split "`n")
    $crlf = $text.Contains("`r`n")
    foreach ($s in $sets) {
        $sec = $s[0]; $key = $s[1]; $val = $s[2]
        $start = -1; $end = $lines.Count
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $t = $lines[$i].Trim()
            if ($t.StartsWith('[')) {
                if ($start -ge 0) { $end = $i; break }
                if ($t.Trim('[', ']').Trim() -ieq $sec) { $start = $i }
            }
        }
        $found = -1
        if ($start -ge 0) {
            for ($i = $start + 1; $i -lt $end; $i++) {
                $t = $lines[$i].Trim()
                if ($t.StartsWith(';') -or $t.StartsWith('#')) { continue }
                $eq = $t.IndexOf('=')
                if ($eq -gt 0 -and $t.Substring(0, $eq).Trim() -ieq $key) { $found = $i; break }
            }
        }
        $show = { param($v) if ($key -match $SecretKey -and $v) { '<secret>' } else { $v } }
        if ($found -ge 0) {
            $line = $lines[$found]; $cr = $line.EndsWith("`r"); if ($cr) { $line = $line.Substring(0, $line.Length - 1) }
            $eq = $line.IndexOf('=')
            $rest = $line.Substring($eq + 1)
            $comment = ''
            $ci = $rest.IndexOf(' ;'); if ($ci -lt 0) { $ci = $rest.IndexOf("`t;") }
            if ($ci -ge 0) { $comment = $rest.Substring($ci); $rest = $rest.Substring(0, $ci) }
            $old = $rest.Trim()
            if ($null -eq $val) {
                $lines.RemoveAt($found); $script:Changes++
                Write-Host ("  {0}: [{1}] {2} removed" -f $label, $sec, $key)
                continue
            }
            if ($old -ceq $val) { continue }
            $lead = if ($rest -match '^\s') { $rest.Substring(0, $rest.Length - $rest.TrimStart().Length) } else { '' }
            $lines[$found] = $line.Substring(0, $eq + 1) + $lead + $val + $comment + $(if ($cr) { "`r" } else { '' })
            $script:Changes++
            Write-Host ("  {0}: [{1}] {2}: {3} -> {4}" -f $label, $sec, $key, (& $show $old), (& $show $val))
            continue
        }
        if ($null -eq $val) { continue }
        $eol = if ($crlf) { "`r" } else { '' }
        if ($start -lt 0) {
            if ($lines.Count -gt 0 -and $lines[$lines.Count - 1].Trim() -ne '') { $lines.Add($eol) }
            $lines.Add("[$sec]$eol"); $start = $lines.Count - 1; $end = $lines.Count
        }
        $at = $start + 1
        for ($i = $start + 1; $i -lt $end; $i++) { if ($lines[$i].Trim() -ne '') { $at = $i + 1 } }
        $lines.Insert($at, "$key=$val$eol")
        $script:Changes++
        Write-Host ("  {0}: [{1}] {2} added = {3}" -f $label, $sec, $key, (& $show $val))
    }
    return ($lines -join "`n")
}

function Save-File([string]$path, [string]$newText, [string]$oldText) {
    if ($oldText -ceq $newText) { return }
    if ($Check) { return }
    if (Test-Path $path) {
        $rel = if ($path.StartsWith($RepoRoot, [StringComparison]::OrdinalIgnoreCase)) { $path.Substring($RepoRoot.Length).TrimStart('\') } else { ($path -replace '[:\\]', '_') }
        $b = Join-Path $BackupDir $rel
        New-Item -ItemType Directory -Force (Split-Path $b) | Out-Null
        Copy-Item $path $b -Force
    } else { New-Item -ItemType Directory -Force (Split-Path $path) | Out-Null }
    [IO.File]::WriteAllText($path, $newText, $Latin1)
}

function Update-Ini([string]$path, $sets, [string]$label) {
    $old = if (Test-Path $path) { [IO.File]::ReadAllText($path, $Latin1) } else { '' }
    if (-not (Test-Path $path)) { Write-Host "  $label : (new file)" }
    $new = Edit-Ini $old $sets $label
    Save-File $path $new $old
}

function Update-Text([string]$path, [string]$newText, [string]$label) {
    $old = if (Test-Path $path) { [IO.File]::ReadAllText($path, $Latin1) } else { '' }
    if ($old -ceq $newText) { return }
    $script:Changes++
    $o = @($old -split "`n" | ForEach-Object { $_.TrimEnd("`r") }); $n = @($newText -split "`n" | ForEach-Object { $_.TrimEnd("`r") })
    Write-Host "  $label :"
    if (-not $old) { Write-Host "     (new file, $($n.Count) lines)"; Save-File $path $newText $old; return }
    foreach ($l in $o) { if ($n -notcontains $l) { Write-Host "     - $l" } }
    foreach ($l in $n) { if ($o -notcontains $l) { Write-Host "     + $l" } }
    Save-File $path $newText $old
}

# ------------------------------------------------------------------ client_config.ini (identical in every folder)
function Build-ClientConfig {
    $existing = @{}
    foreach ($e in @($Games) + @($Realm | Where-Object { $_ })) {
        $f = FolderOf $e; if (-not $f) { continue }
        $p = Join-Path $f 'ServerDataBase\client_config.ini'
        if (Test-Path $p) {
            foreach ($l in [IO.File]::ReadAllLines($p)) { $c = $l.Trim() -split '\s+'; if ($c.Count -ge 11 -and -not $existing.ContainsKey($c[0])) { $existing[$c[0]] = $c } }
        }
    }
    $pipeHost = CfgVal 'PipeHost' '127.0.0.1'
    $rows = @()
    foreach ($e in (@($Games) + @($Realm | Where-Object { $_ }) | Sort-Object { [int]$_.ServerID })) {
        $mid = if ($existing.ContainsKey($e.ServerID)) { $existing[$e.ServerID][2..8] -join ' ' }
               elseif ($e.Section -eq 'Realm') { '3935 0328 0198 0003 0003 1 0' } else { '3935 0222 0217 0001 0000 1 3' }
        $port = if ($e.PipePort) { $e.PipePort } else { 0 }
        $rows += "$($e.ServerID) $($e.Name) $mid $pipeHost $port"
    }
    return ($rows -join "`r`n")
}

# ------------------------------------------------------------------ run
Write-Host ("Master: {0}{1}" -f $Config, $(if ($Check) { '   (CHECK ONLY - nothing is written)' } else { '' }))
$clientConfig = Build-ClientConfig
$realmName = if ($Realm -and $Realm.Name) { $Realm.Name } else { 'Realm1' }

foreach ($e in (@($Games) + @($Realm | Where-Object { $_ }))) {
    if (-not (Want $e.Section)) { continue }
    $folder = FolderOf $e
    if (-not $folder -or -not (Test-Path $folder)) { Write-Host "[$($e.Section)] $($e.Name): folder $folder missing, skipped"; continue }
    $isRealm = $e.Section -eq 'Realm'
    Write-Host "[$($e.Section)] $($e.Name) ($folder)"
    $sets = @(
        @('ServerInfo', 'AddresIP', $(if ($e.Host) { $e.Host } else { $PublicIP })),
        @('ServerInfo', 'Auth_Port', $AuthPort),
        @('ServerInfo', 'ServerName', $e.Name),
        @('ServerInfo', 'ServerID', $e.ServerID),
        @('ServerInfo', 'DbName', (CfgVal 'MySqlDatabase' 'mega')),
        @('ServerInfo', 'DbPass', (CfgVal 'MySqlPassword')),
        @('ServerInfo', 'DbHost', (CfgVal 'MySqlHost' 'localhost')),
        @('ServerInfo', 'DbPort', (CfgVal 'MySqlPort' '3306')),
        @('ServerInfo', 'DbUser', (CfgVal 'MySqlUser' 'root')),
        @('InterServer', 'AddresIP', (CfgVal 'PipeHost' '127.0.0.1')),
        @('InterServer', 'Port', $(if ($e.PipePort) { $e.PipePort } else { '0' })),
        @('InterServer', 'IsInterServer', $(if ($isRealm) { '1' } else { '0' })),
        @('InterServer', 'Secret', (CfgVal 'InterServerSecret')),
        @('InterServer', 'Bind', (CfgVal 'PipeBindIP' '127.0.0.1')),
        @('InterServer', 'RealmName', $realmName)
    )
    if (-not $isRealm) { $sets += , @('ServerInfo', 'Game_Port', $e.GamePort) }
    if ($e.ItemUIDMin) { $sets += , @('Database', 'ItemUIDMin', $e.ItemUIDMin); $sets += , @('Database', 'ItemUIDMax', $e.ItemUIDMax) }
    foreach ($k in 'LoginKey', 'TransferKey', 'TransferSalt') { $v = CfgVal $k; if ($v) { $sets += , @('Security', $k, $v) } }
    Update-Ini (Join-Path $folder 'shell.ini') $sets 'shell.ini'
    Update-Text (Join-Path $folder 'ServerDataBase\client_config.ini') $clientConfig 'client_config.ini'
}

if ($M.Contains('Auth') -and (Want 'Auth')) {
    $a = $M['Auth']; $folder = FolderOf $a
    if ($folder -and (Test-Path $folder)) {
        Write-Host "[Auth] ($folder)"
        $sets = @(
            @('ServerInfo', 'Auth_Port', $AuthPort),
            @('ServerInfo', 'ServerCount', [string]$Published.Count),
            @('MySql', 'Host', (CfgVal 'MySqlHost' 'localhost')),
            @('MySql', 'Port', (CfgVal 'MySqlPort' '3306')),
            @('MySql', 'User', (CfgVal 'MySqlUser' 'root')),
            @('MySql', 'Password', (CfgVal 'MySqlPassword')),
            @('MySql', 'Database', (CfgVal 'MySqlDatabase' 'mega'))
        )
        # fallback list (used only when the AccountServer finds no HazemConfig.ini): the name the client sends
        $n = 0
        foreach ($s in $Published) {
            $n++
            $login = if ($s.LoginName) { $s.LoginName } else { $s.Name }
            $sets += , @('ServerInfo', "Server$n", ("{0},{1},{2},{3}" -f $login, $s.GamePort, $(if ($s.Host) { $s.Host } else { $PublicIP }), $s.ServerID))
        }
        for ($k = $n + 1; $k -le 20; $k++) { $sets += , @('ServerInfo', "Server$k", $null) }
        Update-Ini (Join-Path $folder 'AuthConfig.ini') $sets 'AuthConfig.ini'
    } else { Write-Host "[Auth] folder $folder missing, skipped" }
}

$patchHost = CfgVal 'PatchServerHost' $PublicIP
$patchPort = CfgVal 'PatchServerPort' '8800'
if (Want 'Launcher') {
    foreach ($dir in @((CfgVal 'LauncherFolder' '..\Qahr6609.Tools\Client\Launcher'), (CfgVal 'ClientPatchFolder' '..\Patch'))) {
        $p = Join-Path $Root (Join-Path $dir 'launcher.ini')
        if (-not (Test-Path $p)) { Write-Host "[Launcher] $p missing, skipped"; continue }
        Write-Host "[Launcher] $p"
        Update-Ini $p @(@('Launcher', 'UpdateUrl', "http://${patchHost}:$patchPort/"), @('Launcher', 'ServerIP', $AuthHost), @('Launcher', 'LoginPort', $AuthPort)) 'launcher.ini'
    }
    $p = Join-Path $Root (Join-Path (CfgVal 'PatchServerFolder' '..\Qahr6609.Tools\Server\PatchServer') 'PatchServer.ini')
    if (Test-Path $p) {
        Write-Host "[PatchServer] $p"
        Update-Ini $p @(@('Server', 'Port', $patchPort), @('Servers', 'LoginHost', $AuthHost), @('Servers', 'LoginPort', $AuthPort)) 'PatchServer.ini'
    }
}

if (Want 'Guard') {
    $gf = Join-Path $RepoRoot 'Qahr6609.Tools\Client\Launcher\GuardFile.cs'
    if (Test-Path $gf) {
        if (-not ('ScriptHazem.ClientFiles.GuardFile' -as [type])) { Add-Type -Path $gf }
        $list = New-Object 'System.Collections.Generic.List[ScriptHazem.ClientFiles.GuardFile+Entry]'
        foreach ($s in $Published) {
            $en = New-Object 'ScriptHazem.ClientFiles.GuardFile+Entry'
            $en.LoginName = if ($s.LoginName) { $s.LoginName } else { $s.Name }
            $en.DisplayName = $s.Name
            $en.LoginHost = $AuthHost
            $en.LoginPort = [int]$AuthPort
            $en.Icon = $s.Icon; $en.Hint = $s.Hint
            $list.Add($en)
        }
        $xml = [ScriptHazem.ClientFiles.GuardFile]::BuildXml($list)
        $txt = Join-Path $Root (CfgVal 'GuardTxt' '..\Qahr6609.Tools\Server\Cluster\generated\Guard.txt')
        Write-Host "[Guard] $txt"
        Update-Text $txt ([Text.Encoding]::UTF8.GetString([Text.Encoding]::UTF8.GetBytes($xml)) -replace '[^\x00-\x7F]', '?') 'Guard.txt'
        $dat = Join-Path $Root (Join-Path (CfgVal 'ClientPatchFolder' '..\Patch') 'Guard.dat')
        $current = [ScriptHazem.ClientFiles.GuardFile]::Read($dat)
        $same = $current.Count -eq $list.Count
        for ($i = 0; $same -and $i -lt $list.Count; $i++) {
            $x = $current[$i]; $y = $list[$i]
            if ($x.LoginName -ne $y.LoginName -or $x.LoginHost -ne $y.LoginHost -or $x.LoginPort -ne $y.LoginPort -or $x.DisplayName -ne $y.DisplayName) { $same = $false }
        }
        if (-not $same) {
            Write-Host ("  Guard.dat differs: now {0}" -f (($current | ForEach-Object { "$($_.LoginName)/$($_.DisplayName)@$($_.LoginHost):$($_.LoginPort)" }) -join ', '))
            if ($GuardDat -and -not $Check) {
                $b = Join-Path $BackupDir ((CfgVal 'ClientPatchFolder' '..\Patch') + '\Guard.dat'); New-Item -ItemType Directory -Force (Split-Path $b) | Out-Null
                if (Test-Path $dat) { Copy-Item $dat $b -Force }
                [void][ScriptHazem.ClientFiles.GuardFile]::Write($dat, $list); $script:Changes++
                Write-Host "  Guard.dat written."
            } else { Write-Host "  (Guard.dat is only written with -GuardDat)" }
        }
    } else { Write-Host "[Guard] Launcher\GuardFile.cs missing, skipped" }
}

if ($Check) { Write-Host "`n$($script:Changes) change(s) would be made. Nothing was written." }
elseif ($script:Changes -gt 0) { Write-Host "`n$($script:Changes) change(s) written. Backup: $BackupDir`nRestart the changed processes (Qahr6609.Tools\Server\Cluster\cluster.bat restart <name>) to use them." }
else { Write-Host "`nEverything is already up to date." }
