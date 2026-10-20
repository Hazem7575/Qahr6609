@echo off
rem builds pipetest.exe (pipe v2 security test, see PipeTest.cs header)
"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe" /nologo /platform:anycpu /out:"%~dp0pipetest.exe" "%~dp0PipeTest.cs"
