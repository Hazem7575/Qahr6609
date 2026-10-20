@echo off
rem Re-enables a module disabled with module_off.bat: extracts the newest Qahr6609.GameServer\Modules\Custom\_off\Name*.zip
rem into Qahr6609.GameServer\Modules\Custom\Name\ . The zip is kept (nothing is deleted).
rem     Qahr6609.Tools\Server\ServerCtl\module_on.bat Name
setlocal
if "%~1"=="" (
    echo Usage: %~nx0 ModuleName
    pause
    exit /b 1
)
set "MOD_NAME=%~1"
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
        Write-Host "ERROR: invalid module name '$name'." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    $mods = Join-Path $root 'Qahr6609.GameServer\Modules\Custom'
    $off = Join-Path $mods '_off'
    $dest = Join-Path $mods $name
    if (Test-Path -LiteralPath $dest) {
        Write-Host "ERROR: $dest already exists (module already active). Nothing was changed." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    if (-not (Test-Path -LiteralPath $off -PathType Container)) {
        Write-Host "ERROR: no disabled modules ($off not found)." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    $pattern = '^' + [regex]::Escape($name) + '(_\d{8}_\d{6})?\.zip$'
    $zip = Get-ChildItem -LiteralPath $off -File | Where-Object { $_.Name -match $pattern } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -eq $zip) {
        Write-Host "ERROR: no zip for '$name' in $off" -ForegroundColor Red
        [Environment]::Exit(1)
    }
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    if (Get-Command Expand-Archive -ErrorAction SilentlyContinue) {
        Expand-Archive -LiteralPath $zip.FullName -DestinationPath $dest
    } else {
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [IO.Compression.ZipFile]::ExtractToDirectory($zip.FullName, $dest)
    }
    Write-Host "Module '$name' restored from $($zip.Name) into $dest (the zip was kept)." -ForegroundColor Green
    Write-Host "rebuild with Qahr6609.Tools\Server\ServerCtl\step2_build.bat"
    [Environment]::Exit(0)
} catch {
    Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red
    [Environment]::Exit(1)
}
