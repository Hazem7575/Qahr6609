@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
cd /d "%ROOT%"
set PATH=%PATH%;C:\Program Files\Git\cmd
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\step1.log
echo === git init === > "%LOG%"
if not exist .git git init >> "%LOG%" 2>&1
copy /y "Qahr6609.Tools\Server\ServerCtl\gitignore.txt" ".gitignore" >> "%LOG%" 2>&1
git config user.name "MeGa Dev" >> "%LOG%" 2>&1
git config user.email "mega@localhost" >> "%LOG%" 2>&1
git config core.autocrlf false >> "%LOG%" 2>&1
git add -A >> "%LOG%" 2>&1
git commit -q -m "Snapshot: working server (utf8mb4 fix, client patch IP)" >> "%LOG%" 2>&1
git log --oneline -3 >> "%LOG%" 2>&1
git count-objects -vH >> "%LOG%" 2>&1
echo === export === >> "%LOG%"
if not exist _export mkdir _export
git archive --format=zip -o _export\src.zip HEAD >> "%LOG%" 2>&1
dir _export >> "%LOG%" 2>&1
echo DONE >> "%LOG%"
