@echo off
rem Builds testbot.exe (x86, .NET Framework 4.x) with the framework's csc.exe - no project system needed.
setlocal
cd /d "%~dp0"
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" /nologo /unsafe /optimize+ /platform:x86 /target:exe /out:testbot.exe Program.cs Poker.cs Champ.cs Crypto\*.cs
exit /b %errorlevel%
