@echo off
setlocal
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\build.log
set ERR=%ROOT%\Qahr6609.Tools\Server\ServerCtl\build_errors.log
echo === libs === > "%LOG%"
if not exist "%ROOT%\Qahr6609.GameServer\libs" mkdir "%ROOT%\Qahr6609.GameServer\libs"
for %%F in (BouncyCastle.Crypto.dll DevExpress.Data.Desktop.v21.2.dll DevExpress.Data.v21.2.dll DevExpress.Printing.v21.2.Core.dll DevExpress.Utils.v21.2.dll DevExpress.XtraBars.v21.2.dll DevExpress.XtraEditors.v21.2.dll Extensions.dll) do (
  if not exist "%ROOT%\Qahr6609.GameServer\libs\%%F" copy /y "%ROOT%\Qahr6609.GameServer\bin\Debug\%%F" "%ROOT%\Qahr6609.GameServer\libs\%%F" >> "%LOG%" 2>&1
)
if not exist "%ROOT%\Qahr6609.AccServer\libs" mkdir "%ROOT%\Qahr6609.AccServer\libs"
for %%F in (MySql.Data.dll cityLookUp.dll) do (
  if not exist "%ROOT%\Qahr6609.AccServer\libs\%%F" copy /y "%ROOT%\Qahr6609.AccServer\bin\Debug\%%F" "%ROOT%\Qahr6609.AccServer\libs\%%F" >> "%LOG%" 2>&1
)
dir /b "%ROOT%\Qahr6609.GameServer\libs" "%ROOT%\Qahr6609.AccServer\libs" >> "%LOG%" 2>&1
set VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe
for /f "usebackq delims=" %%M in (`"%VSWHERE%" -latest -prerelease -products * -find MSBuild\**\Bin\MSBuild.exe`) do set MSB=%%M
echo MSBuild: %MSB% >> "%LOG%"
echo === game server (.NET 10 x64) === >> "%LOG%"
rem the local SDK in D:\dotnet10 (the system .NET 10 SDK is broken); output into _buildcheck\game, apply.bat copies it to bin\Debug
set DOTNET_ROOT=D:\dotnet10
if exist "%ROOT%\_buildcheck\game" rmdir /s /q "%ROOT%\_buildcheck\game"
"D:\dotnet10\dotnet.exe" build "%ROOT%\Qahr6609.GameServer\GameServer.csproj" -c Debug -nologo -v minimal -p:OutDir=%ROOT%\_buildcheck\game\ -flp:logfile=%ERR%;errorsonly -flp1:logfile=%ROOT%\Qahr6609.Tools\Server\ServerCtl\build_game_warn.log;warningsonly >> "%LOG%" 2>&1
set GAMEEXIT=%ERRORLEVEL%
echo GAME EXIT %GAMEEXIT% >> "%LOG%"
echo === auth server === >> "%LOG%"
"%MSB%" "%ROOT%\Qahr6609.AccServer\AccServer.csproj" /nologo /v:m /p:Configuration=Debug /p:Platform=AnyCPU /p:OutDir=%ROOT%\_buildcheck\auth\ /flp:logfile=%ROOT%\Qahr6609.Tools\Server\ServerCtl\build_auth_errors.log;errorsonly >> "%LOG%" 2>&1
echo AUTH EXIT %ERRORLEVEL% >> "%LOG%"
echo DONE >> "%LOG%"
