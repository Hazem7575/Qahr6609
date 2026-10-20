@echo off
rem Paths of this repo (computed from this file's folder). CLIENT = the installed 6609 client: edit it if yours is elsewhere.
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
set CLIENT=D:\Conquer Online 3.0
set SDB=%ROOT%\Qahr6609.GameServer\bin\Debug\ServerDataBase
set SRC=%ROOT%\Qahr6609.GameServer
rem ConquerStudio 6609 reads data.wdf / c3.wdf + ini\c3.wdb of the client, Npcs.txt / Monsters of the server data,
rem and the server source (NpcAttribute + AddAvatar) for each NPC's dialog face. Read only: it never writes there.
rem Self-test: ConquerStudio\ConquerStudio.exe --selftest "%CLIENT%" "%SDB%" report.txt
echo Client folder : %CLIENT%
echo Server data   : %SDB%
echo Server source : %SRC%
start "" "%~dp0ConquerStudio\ConquerStudio.exe" --client "%CLIENT%" --db "%SDB%" --source "%SRC%"
