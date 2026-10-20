@echo off
rem ============================================================================================
rem  BUILD.bat - double-click to build both servers (no Visual Studio needed).
rem  Needs: .NET 10 SDK and the .NET Framework 4.8 Developer Pack (see README.md, step 1).
rem  Output:
rem    Qahr6609.GameServer\bin\Debug\GameServer.exe
rem    Qahr6609.AccServer\bin\Debug\AccServer.exe
rem ============================================================================================
setlocal
cd /d "%~dp0"
title Qahr6609 - build

where dotnet >nul 2>nul
if errorlevel 1 (
  echo.
  echo  [X] .NET 10 SDK is not installed.
  echo      Download it from https://dotnet.microsoft.com/download  then run BUILD.bat again.
  echo.
  pause
  exit /b 1
)

echo.
echo  [1/2] Building the game server ...
dotnet build "Qahr6609.GameServer\GameServer.csproj" -c Debug -nologo -v quiet
if errorlevel 1 goto failed

echo.
echo  [2/2] Building the account server ...
dotnet msbuild "Qahr6609.AccServer\AccServer.csproj" -restore -nologo -v:quiet -p:Configuration=Debug -p:Platform=AnyCPU
if errorlevel 1 goto failed

echo.
echo  ============================================================
echo   DONE - both servers are built.
echo     Game server:    Qahr6609.GameServer\bin\Debug\GameServer.exe
echo     Account server: Qahr6609.AccServer\bin\Debug\AccServer.exe
echo  ============================================================
echo.
pause
exit /b 0

:failed
echo.
echo  [X] The build failed. Read the red lines above.
echo      Most common reason: the .NET Framework 4.8 Developer Pack is missing (account server).
echo.
pause
exit /b 1
