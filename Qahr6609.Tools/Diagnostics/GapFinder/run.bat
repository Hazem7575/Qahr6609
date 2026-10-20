@echo off
rem gapfinder: client 6609 vs server gap report (read-only)
cd /d "%~dp0"
chcp 65001 >nul
set PYTHONIOENCODING=utf-8
python gapfinder.py
if errorlevel 1 (
  echo.
  echo gapfinder failed - see the messages above.
  pause
  exit /b 1
)
start "" "%~dp0gapfinder_report.html"
