@echo off
setlocal
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
rem client the new launcher is copied into and started from (was %ROOT%\Patch, moved out 2026-10-20)
set CLIENT=D:\Conquer Online 3.0
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\launcher_build.log
set DOTNET=C:\Program Files\dotnet\dotnet.exe
set VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe
for /f "usebackq delims=" %%M in (`"%VSWHERE%" -latest -prerelease -products * -find MSBuild\**\Bin\MSBuild.exe`) do set MSB=%%M
echo === PatchServer === > "%LOG%"
findstr /C:"_release/" "%ROOT%\.gitignore" >nul || (echo _release/>> "%ROOT%\.gitignore")
"%MSB%" "%ROOT%\Qahr6609.Tools\Server\PatchServer\PatchServer.csproj" /nologo /v:m /p:Configuration=Release >> "%LOG%" 2>&1
echo PATCHSERVER EXIT %ERRORLEVEL% >> "%LOG%"

echo === Launcher (single-file, self-contained) === >> "%LOG%"
if exist "%ROOT%\Qahr6609.Tools\Client\Launcher\obj" rmdir /s /q "%ROOT%\Qahr6609.Tools\Client\Launcher\obj"
if exist "%ROOT%\Qahr6609.Tools\Client\Launcher\bin" rmdir /s /q "%ROOT%\Qahr6609.Tools\Client\Launcher\bin"
set IMG=%ROOT%\Qahr6609.Tools\Client\Launcher\themes\default\images
if not exist "%IMG%\originals" mkdir "%IMG%\originals"
for %%F in (bg1.jpg bg2.jpg bg3.png char1.png char2.png char3.png news1.jpg news2.jpg news3.jpg news4.jpg texture1.png texture2.jpg) do if exist "%IMG%\%%F" move /y "%IMG%\%%F" "%IMG%\originals\%%F" >nul
"%DOTNET%" publish "%ROOT%\Qahr6609.Tools\Client\Launcher\ScriptHazemLauncher.csproj" -c Release -o "%ROOT%\_release" --nologo -v minimal >> "%LOG%" 2>&1
set LEXIT=%ERRORLEVEL%
echo LAUNCHER EXIT %LEXIT% >> "%LOG%"
dir "%ROOT%\_release" >> "%LOG%" 2>&1
if not "%LEXIT%"=="0" goto :end

echo === deploy launcher to client === >> "%LOG%"
taskkill /IM scripthazemLauncher.exe /F >nul 2>&1
timeout /t 2 /nobreak >nul
copy /y "%ROOT%\_release\scripthazemLauncher.exe" "%CLIENT%\scripthazemLauncher.exe" >> "%LOG%" 2>&1
if errorlevel 1 (timeout /t 3 /nobreak >nul & copy /y "%ROOT%\_release\scripthazemLauncher.exe" "%CLIENT%\scripthazemLauncher.exe" >> "%LOG%" 2>&1)
findstr /B /C:"Language=" "%CLIENT%\launcher.ini" >nul || (echo Language=ar>> "%CLIENT%\launcher.ini")
findstr /B /C:"Theme=" "%CLIENT%\launcher.ini" >nul || (echo Theme=default>> "%CLIENT%\launcher.ini")
echo === theme === >> "%LOG%"
robocopy "%ROOT%\Qahr6609.Tools\Client\Launcher\themes" "%CLIENT%\themes" /E /XD originals /NFL /NDL /NJH /NP >> "%LOG%" 2>&1

rem patch files are kept in ClientPatch\files (PatchServer.ini Files=); nothing is seeded into www\files any more
echo === start patch server + launcher === >> "%LOG%"
tasklist /FI "IMAGENAME eq PatchServer.exe" | find /I "PatchServer.exe" >nul || start "PatchServer" /D "%ROOT%\Qahr6609.Tools\Server\PatchServer\bin\Release" "%ROOT%\Qahr6609.Tools\Server\PatchServer\bin\Release\PatchServer.exe"
timeout /t 2 /nobreak >nul
start "" /D "%CLIENT%" "%CLIENT%\scripthazemLauncher.exe"
:end
echo DONE >> "%LOG%"
