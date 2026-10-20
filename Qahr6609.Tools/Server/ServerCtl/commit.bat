@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
set PATH=%PATH%;C:\Program Files\Git\cmd
cd /d "%ROOT%"
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\commit.log
git add -A > "%LOG%" 2>&1
git commit -q -F "Qahr6609.Tools\Server\ServerCtl\commit_msg.txt" >> "%LOG%" 2>&1
git log --oneline -5 >> "%LOG%" 2>&1
git status --short >> "%LOG%" 2>&1
echo DONE >> "%LOG%"
