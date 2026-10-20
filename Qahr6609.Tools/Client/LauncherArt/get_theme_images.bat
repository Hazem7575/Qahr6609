@echo off
for %%I in ("%~dp0..\..\..") do set ROOT=%%~fI
setlocal
set D=%ROOT%\Qahr6609.Tools\Client\Launcher\themes\default\images
set LOG=%ROOT%\Qahr6609.Tools\Server\ServerCtl\images.log
if not exist "%D%" mkdir "%D%"
cd /d "%D%"
echo === download === > "%LOG%"
call :get bg1.jpg "https://images.unsplash.com/photo-1768109027729-cb4ff8455e3d?w=2400&q=85&fm=jpg"
call :get bg2.jpg "https://images.unsplash.com/photo-1787304226680-3b2c2e20dab8?w=2400&q=85&fm=jpg"
call :get bg3.png "https://cdn.pixabay.com/photo/2024/09/12/14/56/temple-9042514_1280.png"
call :get char1.png "https://cdn.pixabay.com/photo/2025/04/28/05/28/ai-generated-9564276_1280.png"
call :get char2.png "https://cdn.pixabay.com/photo/2024/10/24/18/48/man-9146542_1280.png"
call :get char3.png "https://cdn.pixabay.com/photo/2017/06/18/02/14/women-2414360_1280.png"
call :get news1.jpg "https://images.unsplash.com/photo-1513781050488-6dd358209a1b?w=1200&q=85&fm=jpg"
call :get news2.jpg "https://images.unsplash.com/photo-1763823528251-b620cfb737f9?w=1200&q=85&fm=jpg"
call :get news3.jpg "https://cdn.pixabay.com/photo/2025/11/01/23/14/samurai-9931463_1280.jpg"
call :get news4.jpg "https://cdn.pixabay.com/photo/2019/03/23/17/09/fu-4075734_1280.jpg"
call :get texture1.png "https://cdn.pixabay.com/photo/2025/06/02/03/39/pattern-9636142_1280.png"
call :get texture2.jpg "https://cdn.pixabay.com/photo/2022/09/22/02/26/grunge-7471492_1280.jpg"
dir "%D%" >> "%LOG%"
echo DONE >> "%LOG%"
exit /b
:get
curl.exe -s -L -A "Mozilla/5.0" --max-time 60 -o "%~1" -w "%~1 http=%%{http_code} size=%%{size_download} type=%%{content_type}\n" "%~2" >> "%LOG%" 2>&1
exit /b
