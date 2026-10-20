@echo off
rem Creates a new module from Qahr6609.Tools\Server\ServerCtl\module_template:
rem     Qahr6609.Tools\Server\ServerCtl\new_module.bat Name [NpcId]
rem  -> Qahr6609.GameServer\Modules\Custom\Name\NameModule.cs   (then rebuild)
setlocal
if "%~1"=="" (
    echo Usage: %~nx0 ModuleName [NpcId]
    echo   ModuleName: letters, digits, _ ^(starts with a letter^), e.g. DailyGift
    echo   NpcId     : optional NPC UID for the example NPC ^(default: picked automatically, 8160000-8169999^)
    pause
    exit /b 1
)
set "MOD_NAME=%~1"
set "MOD_NPCID=%~2"
set "MOD_ROOT=%~dp0.."
set "MOD_BAT=%~f0"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$f=[IO.File]::ReadAllText($env:MOD_BAT); iex $f.Substring($f.IndexOf('#PS'+'BEGIN'))"
exit /b %errorlevel%

#PSBEGIN  (PowerShell part, run by the line above)
$ErrorActionPreference = 'Stop'
try {
    $name = $env:MOD_NAME
    $root = [IO.Path]::GetFullPath($env:MOD_ROOT)
    if ($name -cnotmatch '^[A-Za-z][A-Za-z0-9_]{1,39}$') {
        Write-Host "ERROR: invalid module name '$name' (letters, digits and _, must start with a letter, 2-40 chars)." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    # names that would hide a namespace / type of the server (module namespace = Conquer6609.Modules.<Name>)
    $reserved = @('Client','Game','Role','Database','Cryptography','ServerSockets','MsgInterServer','Security','Framework',
        'Modules','Core','Properties','WindowsAPI','AdminTool','Bot','BotManager','Program','MyConsole','Extensions','System',
        'Pathfinding','PoP','Project2D','Conquer6609','KernelThread','MapGroupThread','PacketAttribute','ThreadPoll','Catching',
        'JiangHu','Booths','Counter','FlexbilArray','MyMath','BaseFunc','SharedExtensions','ShowChatItems','SendGlobalPacket',
        'ThreadSafeRandom','CachedAttributeInvocation','ErrorCodes','DropMob','Fan','ChatBanned','UnlimitedArenaRooms',
        'AutoHunting','Co_Panle','Template','Help','Commands','Settings')
    if ($reserved -contains $name) {
        Write-Host "ERROR: '$name' is a reserved name (it collides with server code). Choose another name." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    # C# keywords can't be a namespace / class name
    $keywords = @('abstract','as','base','bool','break','byte','case','catch','char','checked','class','const','continue',
        'decimal','default','delegate','do','double','else','enum','event','explicit','extern','false','finally','fixed',
        'float','for','foreach','goto','if','implicit','in','int','interface','internal','is','lock','long','namespace','new',
        'null','object','operator','out','override','params','private','protected','public','readonly','ref','return','sbyte',
        'sealed','short','sizeof','stackalloc','static','string','struct','switch','this','throw','true','try','typeof','uint',
        'ulong','unchecked','unsafe','ushort','using','virtual','void','volatile','while')
    if ($keywords -ccontains $name) {
        Write-Host "ERROR: '$name' is a C# keyword. Choose another name." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    # the example command is @<name in lower case>: it must not take over an existing [GmCommand]
    $cmd = $name.ToLowerInvariant()
    $src = Join-Path $root 'Source'
    if (Test-Path -LiteralPath $src -PathType Container) {
        foreach ($cs in Get-ChildItem -LiteralPath $src -Recurse -File -Filter *.cs) {
            $text = [IO.File]::ReadAllText($cs.FullName)
            if ($text.IndexOf('GmCommand') -lt 0) { continue }
            foreach ($m in [regex]::Matches($text, 'GmCommand\s*\(([^)]*)\)')) {
                # positional arguments (command names) come before the named ones (Role = ..., Help = ...)
                foreach ($arg in $m.Groups[1].Value.Split(',')) {
                    $a = $arg.Trim()
                    if ($a.Contains('=')) { break }
                    if ($a -match '^"([^"]*)"$' -and $Matches[1] -ieq $cmd) {
                        Write-Host "ERROR: the command @$cmd already exists ($($cs.FullName)). Choose another module name." -ForegroundColor Red
                        [Environment]::Exit(1)
                    }
                }
            }
        }
    }
    $tpl = Join-Path $root 'Qahr6609.Tools\Server\ServerCtl\module_template'
    $mods = Join-Path $root 'Qahr6609.GameServer\Modules\Custom'
    $dest = Join-Path $mods $name
    if (-not (Test-Path -LiteralPath $tpl -PathType Container)) {
        Write-Host "ERROR: template folder not found: $tpl" -ForegroundColor Red
        [Environment]::Exit(1)
    }
    if (Test-Path -LiteralPath $dest) {
        Write-Host "ERROR: $dest already exists. Nothing was changed." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    $off = Join-Path $mods '_off'
    if ((Test-Path -LiteralPath $off) -and (Get-ChildItem -LiteralPath $off -File | Where-Object { $_.Name -match ('^' + [regex]::Escape($name) + '(_\d{8}_\d{6})?\.zip$') })) {
        Write-Host "ERROR: a disabled module '$name' exists in $off. Use Qahr6609.Tools\Server\ServerCtl\module_on.bat $name, or delete its zip first." -ForegroundColor Red
        [Environment]::Exit(1)
    }

    # NPC uid for the example NPC handler
    $npc = $env:MOD_NPCID
    $used = @{}
    $npcFile = Join-Path $root 'Qahr6609.GameServer\Networking\Handlers\MsgNpc\NpcID.cs'
    if (Test-Path -LiteralPath $npcFile) {
        foreach ($m in [regex]::Matches([IO.File]::ReadAllText($npcFile), '=\s*(\d+)')) { $used[$m.Groups[1].Value] = 1 }
    }
    if (Test-Path -LiteralPath $mods) {
        foreach ($cs in Get-ChildItem -LiteralPath $mods -Recurse -File -Filter *.cs) {
            foreach ($m in [regex]::Matches([IO.File]::ReadAllText($cs.FullName), 'NpcUid\s*=\s*(\d+)')) { $used[$m.Groups[1].Value] = 1 }
        }
    }
    if ([string]::IsNullOrEmpty($npc)) {
        $h = 0
        foreach ($c in $name.ToLowerInvariant().ToCharArray()) { $h = ($h * 31 + [int]$c) % 10000 }
        $n = 8160000 + $h
        while ($used.ContainsKey([string]$n)) { $n = 8160000 + (($n - 8160000 + 1) % 10000) }
        $npc = [string]$n
    } else {
        $tmp = [uint32]0
        if (-not [uint32]::TryParse($npc, [ref]$tmp) -or $tmp -eq 0) {
            Write-Host "ERROR: NpcId must be a positive number." -ForegroundColor Red
            [Environment]::Exit(1)
        }
        if ($used.ContainsKey($npc)) {
            Write-Host "ERROR: NpcId $npc is already used (NpcID.cs or another module)." -ForegroundColor Red
            [Environment]::Exit(1)
        }
    }

    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    $utf8 = New-Object System.Text.UTF8Encoding($true)
    $count = 0
    foreach ($file in Get-ChildItem -LiteralPath $tpl -Recurse -File) {
        if ($file.Name -ieq 'README.txt') { continue }
        $rel = $file.FullName.Substring($tpl.Length).TrimStart('\', '/').Replace('__NAME__', $name)
        $out = Join-Path $dest $rel
        $dir = Split-Path -Parent $out
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        $text = [IO.File]::ReadAllText($file.FullName)
        $text = $text.Replace('__NAME__', $name).Replace('__name__', $name.ToLowerInvariant()).Replace('__NPCID__', $npc)
        [IO.File]::WriteAllText($out, $text, $utf8)
        $count++
    }
    Write-Host "Module '$name' created in $dest ($count file(s))." -ForegroundColor Green
    Write-Host "  example NPC uid : $npc   (spawn: shell.ini [Module.$name] NpcMap/NpcX/NpcY/NpcLook)"
    Write-Host "  example command: @$($name.ToLowerInvariant())"
    Write-Host "Now rebuild with Qahr6609.Tools\Server\ServerCtl\step2_build.bat"
    [Environment]::Exit(0)
} catch {
    Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red
    [Environment]::Exit(1)
}
