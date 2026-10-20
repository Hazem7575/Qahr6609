@echo off
rem Paths of this repo (computed from this file's folder). CLIENT = the installed 6609 client: edit it if yours is elsewhere.
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
set CLIENT=D:\Conquer Online 3.0
set SDB=%ROOT%\Qahr6609.GameServer\bin\Debug\ServerDataBase
start "" "%~dp0DatCrypt\DatCrypt.exe" --client "%CLIENT%" --db "%SDB%"
