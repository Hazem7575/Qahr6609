@echo off
setlocal
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\deploy_auth.log
set VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe
for /f "usebackq delims=" %%M in (`"%VSWHERE%" -latest -prerelease -products * -find MSBuild\**\Bin\MSBuild.exe`) do set MSB=%%M
echo === build check (no deploy yet) === > "%LOG%"
"%MSB%" "%ROOT%\Qahr6609.AccServer\AccServer.csproj" /nologo /v:m /p:Configuration=Debug /p:Platform=AnyCPU /p:OutDir=%ROOT%\_buildcheck\auth\ >> "%LOG%" 2>&1
if errorlevel 1 (echo BUILD FAILED - auth server left running >> "%LOG%" & goto :end)
echo === stopping auth server === >> "%LOG%"
taskkill /IM AccServer.exe /F >> "%LOG%" 2>&1
timeout /t 2 /nobreak >nul
copy /y "%ROOT%\_buildcheck\auth\AccServer.exe" "%ROOT%\Qahr6609.AccServer\bin\Debug\AccServer.exe" >> "%LOG%" 2>&1
copy /y "%ROOT%\_buildcheck\auth\AccServer.pdb" "%ROOT%\Qahr6609.AccServer\bin\Debug\AccServer.pdb" >> "%LOG%" 2>&1
echo === starting auth server === >> "%LOG%"
cd /d "%ROOT%\Qahr6609.AccServer\bin\Debug"
start "Account Server." AccServer.exe
echo STARTED >> "%LOG%"
:end
echo === game build check === >> "%LOG%"
"%MSB%" "%ROOT%\Qahr6609.GameServer\GameServer.csproj" /nologo /v:q /p:Configuration=Debug /p:Platform=x86 /p:OutDir=%ROOT%\_buildcheck\game\ >> "%LOG%" 2>&1
echo GAME EXIT %ERRORLEVEL% >> "%LOG%"
echo DONE >> "%LOG%"
