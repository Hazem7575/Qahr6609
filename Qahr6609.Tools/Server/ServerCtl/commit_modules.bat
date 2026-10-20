@echo off
rem Commits the modules / performance / security work that is on disk (bin, obj, logs stay ignored).
setlocal
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\commit_modules.log
set PATH=%PATH%;C:\Program Files\Git\cmd
cd /d "%ROOT%"
echo === commit %DATE% %TIME% === > "%LOG%"
git add -A Source AccountServer docs tests .gitignore Qahr6609.Tools/Server/ServerCtl/module_off.bat Qahr6609.Tools/Server/ServerCtl/module_on.bat Qahr6609.Tools/Server/ServerCtl/new_module.bat Qahr6609.Tools/Server/ServerCtl/module_template >> "%LOG%" 2>&1
git commit -q -F "%ROOT%\Qahr6609.Tools\Server\ServerCtl\commit_modules_msg.txt" >> "%LOG%" 2>&1
git log --oneline -3 >> "%LOG%" 2>&1
git status --short >> "%LOG%" 2>&1
echo DONE >> "%LOG%"
endlocal
