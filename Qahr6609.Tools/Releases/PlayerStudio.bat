@echo off
rem Paths of this repo (computed from this file's folder). CLIENT = the installed 6609 client: edit it if yours is elsewhere.
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
set CLIENT=D:\Conquer Online 3.0
set SDB=%ROOT%\Qahr6609.GameServer\bin\Debug\ServerDataBase
rem PlayerStudio has no command line paths: it keeps them in %AppData%\PlayerStudio\settings.json (shared with Qahr's copy).
rem Point that file at this server (itemtype.txt / packages.json / adminapi.txt in SDB, item icons from CLIENT); other settings are kept.
powershell -NoProfile -ExecutionPolicy Bypass -Command "$f = Join-Path $env:APPDATA 'PlayerStudio\settings.json'; $s = $null; if (Test-Path $f) { try { $s = Get-Content -Raw -Encoding UTF8 $f | ConvertFrom-Json } catch {} }; if (-not $s) { $s = [pscustomobject]@{ Port = 5899; Secret = ''; RefreshSeconds = 5; Language = 'ar' } }; $s | Add-Member -Force NoteProperty DbFolder $env:SDB; $s | Add-Member -Force NoteProperty ClientFolder $env:CLIENT; $s | Add-Member -Force NoteProperty UseServerFile $true; New-Item -ItemType Directory -Force (Split-Path $f) | Out-Null; $s | ConvertTo-Json | Set-Content -Encoding UTF8 $f"
start "" "%~dp0PlayerStudio\PlayerStudio.exe"
