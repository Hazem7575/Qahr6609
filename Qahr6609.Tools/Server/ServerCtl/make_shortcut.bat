@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\shortcut.log
powershell -NoProfile -ExecutionPolicy Bypass -Command "$d=[Environment]::GetFolderPath('Desktop'); $s=(New-Object -ComObject WScript.Shell).CreateShortcut((Join-Path $d 'scripthazem.lnk')); $s.TargetPath='D:\Conquer Online 3.0\scripthazemLauncher.exe'; $s.WorkingDirectory='D:\Conquer Online 3.0'; $s.IconLocation='D:\Conquer Online 3.0\scripthazemLauncher.exe,0'; $s.Description='scripthazem launcher'; $s.Save(); 'OK ' + (Join-Path $d 'scripthazem.lnk')" > "%LOG%" 2>&1
