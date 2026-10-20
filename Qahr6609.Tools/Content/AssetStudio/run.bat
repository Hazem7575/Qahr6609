@echo off
setlocal
chcp 65001 >nul
cd /d "%~dp0"
title AssetStudio

where python >nul 2>nul
if errorlevel 1 (
  echo [!] Python not found. Install Python 3 from python.org and tick "Add python to PATH".
  pause
  exit /b 1
)

rem  run.bat           -> extract only the first time, then open the viewer
rem  run.bat update    -> extract again (new client files), keeps already converted images
rem  run.bat force     -> extract again and re-convert every image
rem  run.bat pick      -> choose another client folder first
rem  (every extract also runs extract3d.py = 3D models of the Dressing Room tab, out\3d)
set ARGS=
if /i "%~1"=="pick"  set ARGS=--pick
if /i "%~1"=="force" set ARGS=--force

if not exist "out\data.js" goto extract
if not exist "out\index.html" goto extract
if not "%~1"=="" goto extract
if not exist "out\3d\index.js" goto extract3d
goto open

:extract
echo.
echo === AssetStudio: extracting from the client (first time takes about a minute) ===
python extract.py %ARGS%
if errorlevel 1 (
  echo.
  echo [!] extract failed - see the message above.
  echo     If the client is in another folder:  run.bat pick
  pause
  exit /b 1
)

:extract3d
echo.
echo === AssetStudio: 3D models for the Dressing Room (first time about 2 minutes, later only what changed) ===
set ARGS3=
if /i "%~1"=="force" set ARGS3=--force
python extract3d.py %ARGS3%
if errorlevel 1 (
  echo.
  echo [!] extract3d failed - the item viewer still works, the Dressing Room tab will be empty.
  pause
)

:open
start "" "%~dp0out\index.html"
exit /b 0
