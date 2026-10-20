@echo off
rem Starts the login server if it is not running
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
tasklist /FI "IMAGENAME eq AccServer.exe" | find /I "AccServer.exe" >nul && (echo already running > "%ROOT%\Qahr6609.Tools\Server\ServerCtl\start_auth.log" & goto :eof)
cd /d "%ROOT%\Qahr6609.AccServer\bin\Debug"
start "Account Server." AccServer.exe
echo STARTED > "%ROOT%\Qahr6609.Tools\Server\ServerCtl\start_auth.log"
