@echo off
rem Paths of this repo (computed from this file's folder). CLIENT = the installed 6609 client: edit it if yours is elsewhere.
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
set CLIENT=D:\Conquer Online 3.0
set SDB=%ROOT%\Qahr6609.GameServer\bin\Debug\ServerDataBase
rem MapStudio reads NpcID.cs from <src>\Game\MsgNpc (Qahr's layout): a junction to Networking\Handlers\MsgNpc gives it that path.
set SRC=%~dp0MapStudio\source
if not exist "%SRC%\Game\MsgNpc" (
  mkdir "%SRC%\Game" 2>nul
  mklink /J "%SRC%\Game\MsgNpc" "%ROOT%\Qahr6609.GameServer\Networking\Handlers\MsgNpc" >nul
  mklink /J "%SRC%\GameServer" "%ROOT%\Qahr6609.GameServer" >nul
)
start "" "%~dp0MapStudio\MapStudio.exe" --db "%SDB%" --client "%CLIENT%" --src "%SRC%"
