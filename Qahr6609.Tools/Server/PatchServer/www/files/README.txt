scripthazem PatchServer - files folder
======================================

[English]
Every file placed in this folder (and its sub-folders) is distributed to
players at the same relative path inside their client folder.
Example: files\Env_DX9\Conquer.exe  ->  <client>\Env_DX9\Conquer.exe
The manifest (manifest.json) is rebuilt when the server starts, or when you
type the "rebuild" command in the server console after changing files.
This README.txt itself is not distributed.

[العربية]
أي ملف يوضع في هذا المجلد (ومجلداته الفرعية) يتم توزيعه على اللاعبين
في نفس المسار النسبي داخل مجلد اللعبة لديهم.
مثال: files\Env_DX9\Conquer.exe  ->  <مجلد اللعبة>\Env_DX9\Conquer.exe
يتم إعادة بناء ملف manifest.json عند تشغيل السيرفر، أو بكتابة الأمر
"rebuild" في نافذة السيرفر بعد تغيير الملفات.
ملف README.txt هذا لا يتم توزيعه.
