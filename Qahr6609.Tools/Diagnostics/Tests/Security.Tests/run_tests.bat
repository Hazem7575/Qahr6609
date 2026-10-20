@echo off
set CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe
"%CSC%" /nologo /unsafe /out:"%~dp0SecurityTests.exe" "%~dp0Program.cs" "%~dp0..\..\Qahr6609.GameServer\Game\Security\Guard.cs"
if errorlevel 1 exit /b 1
"%~dp0SecurityTests.exe"
