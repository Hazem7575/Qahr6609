@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
rem ============================================================================
rem  DEV start of the game server with .NET Hot Reload (same as Qahr's dev_hotreload.bat).
rem
rem  Use it ONLY while developing, with the normal game server stopped. A C# edit inside an existing method
rem  body is applied to the running server with NO restart (a magenta "[HotReload] code update applied live"
rem  line shows in the console). A structural edit (new method/field/class, changed signature, static
rem  constructor) is a "rude edit": dotnet watch rebuilds and restarts the server by itself.
rem
rem  This build goes to Qahr6609.GameServer\bin\Dev (unoptimized, Hot Reload needs that) and runs in Qahr6609.GameServer\bin\Debug
rem  (shell.ini, ServerDataBase). The real server (bin\Debug\GameServer.exe, apply.bat) is not touched.
rem  Data files (npcs.txt, shops, itemtype.txt...) have their own live commands: reloadnpcs ... reloadall.
rem
rem  Stop with Ctrl+C in this window.
rem ============================================================================
setlocal
set DOTNET_ROOT=D:\dotnet10
set PATH=D:\dotnet10;%PATH%
set HotReloadDev=true
tasklist /FI "IMAGENAME eq GameServer.exe" | find /I "GameServer.exe" >nul && (
  echo The game server is running ^(GameServer.exe^) - stop it first, both use the same ports and data.
  pause
  exit /b 1
)
cd /d "%ROOT%\Qahr6609.GameServer"
echo.
echo   === Game server DEV (Hot Reload) ===
echo   Edit a method body and save -^> applies live. Structural edit -^> automatic restart.
echo   Ctrl+C here to stop.
echo.
"D:\dotnet10\dotnet.exe" watch --non-interactive --project "GameServer.csproj" -c Debug run
endlocal
