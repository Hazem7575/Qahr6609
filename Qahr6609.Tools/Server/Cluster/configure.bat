@echo off
rem Writes every derived settings file from HazemConfig.ini (repo root). See docs\MULTI_SERVER.md
rem   configure.bat            write (old files backed up to _backup\configure-<time>\)
rem   configure.bat -Check     only show the differences
rem   configure.bat -GuardDat  also write the encrypted client list ClientPatch\files\Guard.dat
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0configure.ps1" %*
