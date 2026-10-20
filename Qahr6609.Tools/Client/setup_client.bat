@echo off
for %%I in ("%~dp0..\..") do set ROOT=%%~fI
echo Copying base client files from EpicGame[V1036] into Patch (existing patch files are kept)...
robocopy "D:\conquer\EpicGame[V1036]" "%ROOT%\Patch" /E /XC /XN /XO /MT:8 /R:1 /W:1 /NFL /NDL /NP
echo.
echo DONE (robocopy exit code %ERRORLEVEL%)
echo DONE> "%ROOT%\setup_client.done"
