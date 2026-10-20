@echo off
rem Paths of this repo (computed from this file's folder). CLIENT = the installed 6609 client: edit it if yours is elsewhere.
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
set CLIENT=D:\Conquer Online 3.0
set SDB=%ROOT%\Qahr6609.GameServer\bin\Debug\ServerDataBase
rem UiExplorer: set "Client folder" = %CLIENT% and "Server data folder" = %SDB% in its settings once
rem (it reads packetstream.txt / packetinject.txt there; the server must be in development mode).
echo Client folder : %CLIENT%
echo Server data   : %SDB%
start "" "%~dp0UiExplorer\UiExplorer.exe"