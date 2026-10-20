@echo off
rem Hazem cluster control panel - see docs\MULTI_SERVER.md
rem   cluster.bat                 status
rem   cluster.bat start all       MySQL check, AccServer, Realm, game servers (Enabled=1 in servers.ini)
rem   cluster.bat stop Hazem      graceful stop (players saved); add -Force to kill an old exe without admin pipe
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0cluster.ps1" %*
