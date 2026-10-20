@echo off
rem ===================================================================
rem  apply.bat - one script that applies a prepared change set.
rem  Changed files go in Qahr6609.Tools\Server\ServerCtl\apply\root\ (same folders as %ROOT%)
rem  and Qahr6609.Tools\Server\ServerCtl\apply\epic\ (same folders as the EpicGame client), plus commit_msg.txt.
rem  This script: backs up the files it replaces, copies the new ones,
rem  builds + restarts the game server only when source code changed,
rem  restarts the patch server, commits to git, then empties the apply folder.
rem  Log: Qahr6609.Tools\Server\ServerCtl\apply.log
rem ===================================================================
setlocal EnableDelayedExpansion
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set EPIC=D:\conquer\EpicGame[V1036]
set BIN=%ROOT%\Qahr6609.GameServer\bin\Debug
set PS=%ROOT%\Qahr6609.Tools\Server\PatchServer
set A=%ROOT%\Qahr6609.Tools\Server\ServerCtl\apply
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\apply.log
set PATH=%PATH%;C:\Program Files\Git\cmd
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd-HHmmss"') do set STAMP=%%i
set BK=%ROOT%\_backup\apply-%STAMP%
echo === apply %STAMP% === > "%LOG%"
if not exist "%A%\root" if not exist "%A%\epic" (echo nothing to apply >> "%LOG%" & goto :end)

rem --- one-time tidy: old one-off scripts go to Qahr6609.Tools\_Archive\old
if not exist "%ROOT%\Qahr6609.Tools\_Archive\old" mkdir "%ROOT%\Qahr6609.Tools\_Archive\old"
for %%f in (fix2 fix3 fix4 fix5 gm_fix shield_fix) do (
  if exist "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f.bat" move /y "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f.bat" "%ROOT%\Qahr6609.Tools\_Archive\old\" >nul
  if exist "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f.log" move /y "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f.log" "%ROOT%\Qahr6609.Tools\_Archive\old\" >nul
  if exist "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f_commit_msg.txt" move /y "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f_commit_msg.txt" "%ROOT%\Qahr6609.Tools\_Archive\old\" >nul
  if exist "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f" move /y "%ROOT%\Qahr6609.Tools\Server\ServerCtl\%%f" "%ROOT%\Qahr6609.Tools\_Archive\old\" >nul
)

echo === backup to %BK% === >> "%LOG%"
powershell -NoProfile -Command "foreach($p in @(@('%A%\root','%ROOT%','%BK%\root'),@('%A%\epic','%EPIC%','%BK%\epic'))){ if(Test-Path $p[0]){ Get-ChildItem $p[0] -Recurse -File | ForEach-Object { $rel=$_.FullName.Substring($p[0].Length+1); $t=Join-Path $p[1] $rel; if(Test-Path $t){ $b=Join-Path $p[2] $rel; New-Item -ItemType Directory -Force (Split-Path $b) | Out-Null; Copy-Item $t $b -Force; 'backup '+$rel } else { 'new    '+$rel } } } }" >> "%LOG%" 2>&1

if exist "%A%\root\Qahr6609.GameServer\GameServer.csproj" del /q "%A%\root\Qahr6609.GameServer\GameServer.csproj"
rem --- project file: never overwritten (other work may edit it). It globs every .cs (.NET 10 SDK project), so a new
rem     file needs no project line any more; csproj_add.txt is ignored.
if exist "%A%\csproj_add.txt" echo csproj_add.txt ignored: the .NET 10 project compiles every .cs file >> "%LOG%"
set BUILD=0
dir /s /b "%A%\root\Qahr6609.GameServer\*.cs" >nul 2>&1 && set BUILD=1

echo === copy === >> "%LOG%"
if exist "%A%\root" xcopy "%A%\root\*" "%ROOT%\" /s /y /i /q >> "%LOG%" 2>&1
if exist "%A%\epic" xcopy "%A%\epic\*" "%EPIC%\" /s /y /i /q >> "%LOG%" 2>&1

if "%BUILD%"=="1" (
  echo === build === >> "%LOG%"
  call "%ROOT%\Qahr6609.Tools\Server\ServerCtl\step2_build.bat"
  findstr /C:"GAME EXIT" "%ROOT%\Qahr6609.Tools\Server\ServerCtl\build.log" >> "%LOG%"
  type "%ROOT%\Qahr6609.Tools\Server\ServerCtl\build_errors.log" >> "%LOG%" 2>&1
  findstr /C:"GAME EXIT 0" "%ROOT%\Qahr6609.Tools\Server\ServerCtl\build.log" >nul || (
    echo ABORT build failed - restoring the replaced files >> "%LOG%"
    if exist "%BK%\root" xcopy "%BK%\root\*" "%ROOT%\" /s /y /i /q >> "%LOG%" 2>&1
    if exist "%BK%\epic" xcopy "%BK%\epic\*" "%EPIC%\" /s /y /i /q >> "%LOG%" 2>&1
    goto :end
  )
)

echo === patch server (rebuild manifest) === >> "%LOG%"
taskkill /IM PatchServer.exe /F >> "%LOG%" 2>&1
timeout /t 2 /nobreak >nul
start "PatchServer" /D "%PS%\bin\Release" "%PS%\bin\Release\PatchServer.exe"

echo === game server === >> "%LOG%"
if "%BUILD%"=="1" (
  copy /y "%BIN%\GameServer.exe" "%BK%\GameServer.exe.old" >nul 2>&1
  copy /y "%BIN%\GameServer.dll" "%BK%\GameServer.dll.old" >nul 2>&1
)
taskkill /IM GameServer.exe /F >> "%LOG%" 2>&1
timeout /t 3 /nobreak >nul
if "%BUILD%"=="1" (
  rem .NET 10: GameServer.exe is only the launcher; the code is GameServer.dll + its package dlls/json files
  xcopy "%ROOT%\_buildcheck\game\*" "%BIN%\" /s /y /i /q >> "%LOG%" 2>&1
)
rem --- optional one-off step while the game server is stopped (e.g. a shell.ini key)
if exist "%A%\pre_start.bat" call "%A%\pre_start.bat" >> "%LOG%" 2>&1
start "GameServer" /D "%BIN%" "%BIN%\GameServer.exe"
echo SERVER STARTED >> "%LOG%"

echo === git === >> "%LOG%"
cd /d "%ROOT%"
git add -A Qahr6609.Tools\Server\ServerCtl\apply.bat Qahr6609.Tools\_Archive\old >> "%LOG%" 2>&1
if exist "%A%\commit_msg.txt" (git commit -q -F "%A%\commit_msg.txt" >> "%LOG%" 2>&1) else (git commit -q -m "apply %STAMP%" >> "%LOG%" 2>&1)
git log --oneline -1 >> "%LOG%" 2>&1

rem --- empty the apply folder so the next change starts clean
rmdir /s /q "%A%" >> "%LOG%" 2>&1
:end
echo DONE >> "%LOG%"
