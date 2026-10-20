@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
setlocal
set OUT=%ROOT%\_art_src
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\download_art.log
if not exist "%OUT%" mkdir "%OUT%"
echo Pixabay images (Pixabay Content License - free for commercial use) > "%LOG%"
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\char_8623750.png" "https://cdn.pixabay.com/photo/2024/03/10/02/33/ai-generated-8623750_1280.png" >> "%LOG%" 2>&1
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\ninja_8937619.jpg" "https://cdn.pixabay.com/photo/2024/08/01/16/04/samurai-8937619_1280.jpg" >> "%LOG%" 2>&1
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\red_8083208.png" "https://cdn.pixabay.com/photo/2023/06/23/10/33/ai-generated-8083208_1280.png" >> "%LOG%" 2>&1
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\hood_8266005.png" "https://cdn.pixabay.com/photo/2023/09/21/05/20/ai-generated-8266005_1280.png" >> "%LOG%" 2>&1
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\bg_9023008.png" "https://cdn.pixabay.com/photo/2024/09/04/22/50/ai-generated-9023008_1280.png" >> "%LOG%" 2>&1
curl -sS -L -A "Mozilla/5.0" -o "%OUT%\castle_8800856.jpg" "https://cdn.pixabay.com/photo/2024/05/31/15/50/ai-generated-8800856_1280.jpg" >> "%LOG%" 2>&1
dir "%OUT%" >> "%LOG%" 2>&1
echo DONE >> "%LOG%"
