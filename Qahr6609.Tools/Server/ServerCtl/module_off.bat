@echo off
rem Disables a module: zips Qahr6609.GameServer\Modules\Custom\Name into Qahr6609.GameServer\Modules\Custom\_off\Name.zip, then deletes the folder.
rem     Qahr6609.Tools\Server\ServerCtl\module_off.bat Name
rem Re-enable with Qahr6609.Tools\Server\ServerCtl\module_on.bat Name. Rebuild after both.
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
    $src = Join-Path $mods $name
    if (-not (Test-Path -LiteralPath $src -PathType Container)) {
        Write-Host "ERROR: module folder not found: $src" -ForegroundColor Red
        [Environment]::Exit(1)
    }
    $items = @(Get-ChildItem -LiteralPath $src -Force)
    if ($items.Count -eq 0) {
        Write-Host "ERROR: $src is empty, nothing to zip. Delete the folder by hand if you want." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    $off = Join-Path $mods '_off'
    if (-not (Test-Path -LiteralPath $off)) { New-Item -ItemType Directory -Path $off -Force | Out-Null }
    $zip = Join-Path $off ($name + '.zip')
    if (Test-Path -LiteralPath $zip) {
        $zip = Join-Path $off ($name + '_' + (Get-Date -Format 'yyyyMMdd_HHmmss') + '.zip')
    }
    # ZipFile (not Compress-Archive: that one skips hidden files and fails on some names)
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [IO.Compression.ZipFile]::CreateFromDirectory($src, $zip)
    # verify: every file of the folder must be in the zip with the same size, else the folder is kept
    $missing = @()
    $archive = [IO.Compression.ZipFile]::OpenRead($zip)
    try {
        $entries = @{}
        foreach ($e in $archive.Entries) { $entries[$e.FullName.Replace('\', '/').ToLowerInvariant()] = $e.Length }
        $srcFull = (Get-Item -LiteralPath $src -Force).FullName.TrimEnd('\', '/')
        foreach ($f in @(Get-ChildItem -LiteralPath $src -Recurse -File -Force)) {
            $rel = $f.FullName.Substring($srcFull.Length).TrimStart('\', '/').Replace('\', '/').ToLowerInvariant()
            if (-not $entries.ContainsKey($rel) -or $entries[$rel] -ne $f.Length) { $missing += $f.FullName }
        }
    } finally {
        $archive.Dispose()
    }
    if ($missing.Count -gt 0) {
        Write-Host "ERROR: the zip $zip does not match the module folder ($($missing.Count) file(s) missing or different, e.g. $($missing[0]))." -ForegroundColor Red
        Write-Host "The module folder was NOT deleted." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    try {
        Remove-Item -LiteralPath $src -Recurse -Force
    } catch {
        Write-Host ("ERROR: could not delete the module folder: " + $_.Exception.Message) -ForegroundColor Red
        Write-Host "The zip is saved at $zip . Delete the folder $src by hand, then rebuild." -ForegroundColor Red
        [Environment]::Exit(1)
    }
    Write-Host "Module '$name' disabled: saved to $zip and removed from Qahr6609.GameServer\Modules\Custom." -ForegroundColor Green
    Write-Host "Its data/logs in <DbLocation>\Modules\$name\ were kept."
    Write-Host "rebuild with Qahr6609.Tools\Server\ServerCtl\step2_build.bat"
    [Environment]::Exit(0)
} catch {
    Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red
    [Environment]::Exit(1)
}
