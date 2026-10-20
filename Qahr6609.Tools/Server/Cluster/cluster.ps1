<#
  Hazem cluster control panel (docs\MULTI_SERVER.md).
  Reads servers.ini and starts / stops / watches each process in its own folder:
    MySQL (service check) -> [Auth] -> [Realm] (Enabled=1) -> [ServerN] (Enabled=1); stop in reverse order.

  Usage (PowerShell 5.1):
    cluster.ps1                       status of every entry
    cluster.ps1 status
    cluster.ps1 start   all|auth|realm|<Name>|<ServerID>|<Section>
    cluster.ps1 stop    all|...  [-Force]
    cluster.ps1 restart <target> [-Force]
    cluster.ps1 save    <target>         game/realm: save database + online characters
    cluster.ps1 stop-path -Path <full exe path> [-Force]   used by Qahr6609.Tools\Server\ServerCtl\apply.bat (one process, by its path)
    cluster.ps1 watch                    status every 5 s (Ctrl+C to leave)
    cluster.ps1 logs    <target> [-Lines 40]   last lines of each server's log (one window for everything)
  Windows of started processes: [Global] ConsoleMode=normal|minimized|hidden (default minimized), or -ConsoleMode.
  Options: -Registry <servers.ini>  (default: repo root servers.ini)   -TimeoutSec <n> (stop wait, default 180)

  Stop = graceful: the game server's admin pipe \\.\pipe\hazem-admin-<pid> gets "exit" (players are disconnected and
  saved, like Ctrl+C in its console), then the panel waits for the process to end. A server without the admin pipe
  (an exe older than this panel) is only killed with -Force. AuthServer keeps no state: it is killed.
  Processes are found by their executable path, never by image name: other servers / sandboxes are never touched.
#>
param(
    [Parameter(Position = 0)][string]$Action = 'status',
    [Parameter(Position = 1)][string]$Target = 'all',
    [string]$Registry = '',
    [string]$Path = '',
    [switch]$Force,
    [string]$ConsoleMode = '',
    [int]$Lines = 40,
    [int]$TimeoutSec = 180
)
$ErrorActionPreference = 'Stop'
if (-not $Registry) {
    $repo = Split-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) -Parent
    $Registry = Join-Path $repo 'Qahr6609.Shared\HazemConfig.ini'
    if (-not (Test-Path $Registry) -and (Test-Path (Join-Path $repo 'servers.ini'))) { $Registry = Join-Path $repo 'servers.ini' }
}

function Read-Registry([string]$file) {
    if (-not (Test-Path $file)) { throw "master file not found: $file (HazemConfig.ini, docs\MULTI_SERVER.md)" }
    $root = Split-Path (Resolve-Path $file) -Parent
    $sections = [ordered]@{}
    $cur = $null
    foreach ($raw in [IO.File]::ReadAllLines($file)) {
        $line = $raw
        $i = $line.IndexOf(';'); if ($i -ge 0) { $line = $line.Substring(0, $i) }
        $line = $line.Trim()
        if (-not $line) { continue }
        if ($line.StartsWith('[')) { $cur = $line.Trim('[', ']').Trim(); $sections[$cur] = @{ Section = $cur }; continue }
        if ($null -eq $cur) { continue }
        $eq = $line.IndexOf('='); if ($eq -le 0) { continue }
        $sections[$cur][$line.Substring(0, $eq).Trim()] = $line.Substring($eq + 1).Trim()
    }
    $entries = @()
    foreach ($name in $sections.Keys) {
        $s = $sections[$name]
        $role = if ($name -eq 'Auth') { 'auth' } elseif ($name -eq 'Realm') { 'realm' } elseif ($name -like 'Server*') { 'game' } else { $null }
        if (-not $role) { continue }
        $folder = if ($s.Folder) { if ([IO.Path]::IsPathRooted($s.Folder)) { $s.Folder } else { Join-Path $root $s.Folder } } else { $null }
        $exe = if ($s.Exe) { $s.Exe } elseif ($role -eq 'auth') { 'AccServer.exe' } else { 'GameServer.exe' }
        $port = 0
        if ($role -eq 'auth') { $port = [int]$s.Port } elseif ($role -eq 'game') { $port = [int]$s.GamePort } else { $port = [int]$s.PipePort }
        $order = if ($role -eq 'auth') { 1 } elseif ($role -eq 'realm') { 2 } else { 3 }
        $entries += [pscustomobject]@{
            Section  = $name; Role = $role; Order = $order
            Name     = if ($s.Name) { $s.Name } else { $name }
            ServerID = [int]$s.ServerID
            Folder   = if ($folder) { [IO.Path]::GetFullPath($folder) } else { $null }
            Exe      = $exe
            ExePath  = if ($folder) { [IO.Path]::GetFullPath((Join-Path $folder $exe)) } else { $null }
            Port     = $port
            Enabled  = ($s.Enabled -eq '1')
            Published = ($s.Published -eq '1')
        }
    }
    # MySQL settings: [Global] MySqlService/MySqlPort (HazemConfig.ini) or [Cluster] (older servers.ini)
    $cluster = @{}
    foreach ($sec in 'Cluster', 'Global') { if ($sections.Contains($sec)) { foreach ($k in 'MySqlService', 'MySqlPort', 'ConsoleMode') { if ($sections[$sec][$k]) { $cluster[$k] = $sections[$sec][$k] } } } }
    return [pscustomobject]@{ Root = $root; Entries = $entries; Cluster = $cluster }
}

function Get-ProcByPath([string]$exePath) {
    if (-not $exePath) { return @() }
    $name = [IO.Path]::GetFileName($exePath).Replace("'", "''")
    return @(Get-CimInstance Win32_Process -Filter "Name='$name'" | Where-Object { $_.ExecutablePath -and ($_.ExecutablePath -ieq $exePath) })
}

function Test-Port([int]$port, [string]$hostName = '127.0.0.1') {
    if ($port -le 0) { return $null }
    $c = New-Object Net.Sockets.TcpClient
    try { $ar = $c.BeginConnect($hostName, $port, $null, $null); if (-not $ar.AsyncWaitHandle.WaitOne(700)) { return $false }; $c.EndConnect($ar); return $true }
    catch { return $false } finally { $c.Close() }
}

function Invoke-Admin([int]$procId, [string]$cmd, [int]$timeoutMs = 3000) {
    $pipe = New-Object IO.Pipes.NamedPipeClientStream('.', "hazem-admin-$procId", [IO.Pipes.PipeDirection]::InOut)
    try {
        $pipe.Connect($timeoutMs)
        $w = New-Object IO.StreamWriter($pipe, [Text.Encoding]::ASCII); $w.AutoFlush = $true
        $w.Write("$cmd`n")
        $r = New-Object IO.StreamReader($pipe, [Text.Encoding]::ASCII)
        $task = $r.ReadLineAsync()
        $wait = if ($cmd -eq 'save') { 180000 } else { $timeoutMs }
        if ($task.Wait($wait)) { return $task.Result } else { return $null }
    } catch { return $null } finally { $pipe.Dispose() }
}

function Write-Pid($e, [int]$procId) {
    if ($e.Folder) { try { Set-Content -Path (Join-Path $e.Folder 'server.pid') -Value $procId -Encoding ascii } catch { } }
}
function Remove-Pid($e) {
    if ($e.Folder) { $p = Join-Path $e.Folder 'server.pid'; if (Test-Path $p) { Remove-Item $p -Force -ErrorAction SilentlyContinue } }
}

function Select-Targets($reg, [string]$t) {
    $all = $reg.Entries
    if ($t -eq 'all') { return @($all | Where-Object { $_.Enabled }) }
    $m = @($all | Where-Object { $_.Section -ieq $t -or $_.Name -ieq $t -or $_.Role -ieq $t -or ($t -match '^\d+$' -and $_.ServerID -eq [int]$t -and $_.Role -ne 'auth') })
    if ($m.Count -eq 0) { throw "no entry '$t' in $Registry" }
    return $m
}

function Show-Status($reg) {
    $rows = foreach ($e in ($reg.Entries | Sort-Object Order, ServerID)) {
        $procs = @(Get-ProcByPath $e.ExePath)
        $p = $procs | Select-Object -First 1
        $ram = ''; $up = ''; $info = ''
        if ($p) {
            try { $gp = Get-Process -Id $p.ProcessId; $ram = [int]($gp.WorkingSet64 / 1MB); $up = ((Get-Date) - $gp.StartTime).ToString('d\.hh\:mm\:ss') } catch { }
            if ($e.Role -ne 'auth') { $a = Invoke-Admin $p.ProcessId 'ping' 800; $info = if ($a) { ($a -replace '^ok ', '') } else { 'no admin pipe' } }
            if ($ram -and [int]$ram -gt 3200) { $info = "RAM > 3.2 GB (32-bit limit near) $info" }
        }
        [pscustomobject]@{
            Entry   = $e.Section; Name = $e.Name; Id = $e.ServerID; Role = $e.Role
            Enabled = if ($e.Enabled) { 'yes' } else { 'no' }
            PID     = if ($p) { ($procs.ProcessId -join ',') } else { '-' }
            Port    = if ($e.Port) { "$($e.Port) " + $(if (Test-Port $e.Port) { 'open' } else { 'closed' }) } else { '' }
            RAM_MB  = $ram; Uptime = $up; Info = $info
        }
    }
    $svc = $reg.Cluster['MySqlService']
    if ($svc) { $s = Get-Service -Name $svc -ErrorAction SilentlyContinue; Write-Host ("MySQL service {0}: {1}" -f $svc, $(if ($s) { $s.Status } else { 'not found' })) }
    $rows | Format-Table -AutoSize | Out-String -Width 220 | Write-Host
}

function Assert-MySql($reg) {
    $svc = $reg.Cluster['MySqlService']
    if ($svc) {
        $s = Get-Service -Name $svc -ErrorAction SilentlyContinue
        if (-not $s -or $s.Status -ne 'Running') { throw "MySQL service '$svc' is not running - start it first." }
    }
    $port = [int]$reg.Cluster['MySqlPort']
    if ($port -gt 0 -and -not (Test-Port $port)) { throw "MySQL port $port is not open." }
}

function Start-Entry($e) {
    if (-not $e.ExePath -or -not (Test-Path $e.ExePath)) { Write-Host "[$($e.Section)] exe not found: $($e.ExePath)"; return }
    $running = @(Get-ProcByPath $e.ExePath)
    if ($running.Count -gt 0) { Write-Host "[$($e.Section)] $($e.Name) already running (PID $($running.ProcessId -join ','))"; return }
    if ($e.Port -gt 0 -and (Test-Port $e.Port)) { Write-Host "[$($e.Section)] port $($e.Port) is already in use by another process - not started"; return }
    # [Global] ConsoleMode=normal|minimized|hidden (default minimized); -ConsoleMode overrides. The console is never
    # redirected (the server UI needs a real console); its log is <folder>\Exceptions\<date>.log ("logs" command).
    $mode = if ($ConsoleMode) { $ConsoleMode } elseif ($script:Reg.Cluster['ConsoleMode']) { $script:Reg.Cluster['ConsoleMode'] } else { 'minimized' }
    $style = switch ($mode.ToLowerInvariant()) { 'normal' { 'Normal' } 'hidden' { 'Hidden' } default { 'Minimized' } }
    $p = Start-Process -FilePath $e.ExePath -WorkingDirectory $e.Folder -WindowStyle $style -PassThru
    Write-Pid $e $p.Id
    Write-Host "[$($e.Section)] $($e.Name) started, PID $($p.Id), waiting until it is up..."
    # one at a time: the next process starts only when this one finished loading (avoids MySQL connection storms)
    $deadline = (Get-Date).AddSeconds(180)
    while ((Get-Date) -lt $deadline) {
        if ($p.HasExited) { Write-Host "[$($e.Section)] process exited (code $($p.ExitCode)) - see its console / Exceptions folder"; return }
        if ($e.Role -eq 'auth') { if (Test-Port $e.Port) { Write-Host "[$($e.Section)] port $($e.Port) open."; return } }
        else {
            $a = Invoke-Admin $p.Id 'ping' 500
            if ($a -and $a -match 'loaded=1') {
                $portState = if ($e.Port -gt 0) { if (Test-Port $e.Port) { "port $($e.Port) open" } else { "port $($e.Port) CLOSED" } } else { '' }
                Write-Host "[$($e.Section)] loaded ($portState): $a"; return
            }
        }
        Start-Sleep -Milliseconds 700
    }
    Write-Host "[$($e.Section)] not up after 180 s (check its console window / Exceptions folder)." 
}

function Stop-Proc($e, [int]$procId, [string]$role) {
    if ($role -ne 'auth') {
        $reply = Invoke-Admin $procId 'exit' 3000
        if ($reply -eq 'stopping') {
            Write-Host "[$($e.Section)] graceful stop requested (players saved), waiting up to $TimeoutSec s..."
            $deadline = (Get-Date).AddSeconds($TimeoutSec)
            while ((Get-Date) -lt $deadline) {
                if (-not (Get-Process -Id $procId -ErrorAction SilentlyContinue)) { Write-Host "[$($e.Section)] stopped."; return $true }
                Start-Sleep -Milliseconds 500
            }
            Write-Host "[$($e.Section)] still running after $TimeoutSec s."
        } else {
            Write-Host "[$($e.Section)] PID $procId has no admin pipe (older exe or [Admin] Pipe=0)."
        }
        if (-not $Force) { Write-Host "[$($e.Section)] not killed: type 'exit' in its console, or use -Force."; return $false }
    }
    Stop-Process -Id $procId -Force -ErrorAction SilentlyContinue
    Write-Host "[$($e.Section)] PID $procId killed."
    return $true
}

function Stop-Entry($e) {
    $procs = @(Get-ProcByPath $e.ExePath)
    if ($procs.Count -eq 0) { Write-Host "[$($e.Section)] $($e.Name) not running."; Remove-Pid $e; return }
    foreach ($p in $procs) { [void](Stop-Proc $e $p.ProcessId $e.Role) }
    if (@(Get-ProcByPath $e.ExePath).Count -eq 0) { Remove-Pid $e }
}

switch ($Action.ToLowerInvariant()) {
    'stop-path' {
        if (-not $Path) { throw '-Path <exe> is required' }
        $full = [IO.Path]::GetFullPath($Path)
        $role = if ([IO.Path]::GetFileName($full) -ieq 'AccServer.exe') { 'auth' } else { 'game' }
        $e = [pscustomobject]@{ Section = [IO.Path]::GetFileName((Split-Path $full -Parent)); Folder = (Split-Path $full -Parent); ExePath = $full; Role = $role }
        $procs = @(Get-ProcByPath $full)
        if ($procs.Count -eq 0) { Write-Host "no process runs $full"; exit 0 }
        foreach ($p in $procs) { [void](Stop-Proc $e $p.ProcessId $role) }
        if (@(Get-ProcByPath $full).Count -gt 0) { exit 1 }
        exit 0
    }
    'status' { Show-Status (Read-Registry $Registry) }
    'logs' {
        # last lines of each server's console log (Exceptions\<date>.log; AuthServer: Security_Logs)
        $reg = Read-Registry $Registry
        foreach ($e in (Select-Targets $reg $Target)) {
            $dir = if ($e.Role -eq 'auth') { Join-Path $e.Folder 'Security_Logs' } else { Join-Path $e.Folder 'Exceptions' }
            $f = Get-ChildItem $dir -File -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
            Write-Host "===== [$($e.Section)] $($e.Name): $(if ($f) { $f.FullName } else { 'no log yet' })"
            if ($f) { Get-Content $f.FullName -Tail $Lines | Write-Host }
        }
    }
    'watch' { $reg = Read-Registry $Registry; while ($true) { Clear-Host; Write-Host (Get-Date); Show-Status $reg; Start-Sleep 5 } }
    'start' {
        $reg = Read-Registry $Registry; $script:Reg = $reg
        Assert-MySql $reg
        foreach ($e in (Select-Targets $reg $Target | Sort-Object Order, ServerID)) { Start-Entry $e }
    }
    'stop' {
        $reg = Read-Registry $Registry
        foreach ($e in (Select-Targets $reg $Target | Sort-Object Order, ServerID -Descending)) { Stop-Entry $e }
    }
    'restart' {
        $reg = Read-Registry $Registry; $script:Reg = $reg
        $t = Select-Targets $reg $Target
        foreach ($e in ($t | Sort-Object Order, ServerID -Descending)) { Stop-Entry $e }
        Assert-MySql $reg
        foreach ($e in ($t | Sort-Object Order, ServerID)) { Start-Entry $e }
    }
    'save' {
        $reg = Read-Registry $Registry
        foreach ($e in (Select-Targets $reg $Target | Where-Object { $_.Role -ne 'auth' })) {
            foreach ($p in (Get-ProcByPath $e.ExePath)) { Write-Host "[$($e.Section)] save: $(Invoke-Admin $p.ProcessId 'save' 3000)" }
        }
    }
    default { throw "unknown action '$Action' (status|start|stop|restart|save|watch|stop-path)" }
}
