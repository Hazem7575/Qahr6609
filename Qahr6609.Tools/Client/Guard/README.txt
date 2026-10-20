guardtool - the client login server list (Guard.dat)
=====================================================

The 6609 client used by Hazem reads its login-screen server list from Guard.dat in the client root
(Server.dat next to it is TQ's untouched 2017 file and is not used for our list).

  guardtool encode Guard.txt Guard.dat    plain XML  -> client file
  guardtool decode Guard.dat Guard.txt    client file -> plain XML
  guardtool verify Guard.txt Guard.dat    exit 0 when they match byte for byte (both ways)
  guardtool list   Guard.dat              print the server rows

Exit codes: 0 ok, 1 usage, 2 error, 3 verify mismatch. Paths may be relative to the current folder.
Build: MSBuild guardtool.csproj /p:Configuration=Release (writes guardtool.exe here, .NET Framework 4.8).
The code is Launcher\GuardFile.cs (shared with the launcher and PatchServer).

Cipher (from "أداة تعديل اسم واي بي السيرفر\اسحب الملف هنا.exe", class CServerFile)
-----------------------------------------------------------------------------------
Byte-wise XOR, same size in and out, the same function encodes and decodes:

    key (32 bytes) = 5d cf 12 f6 41 0b 9e e0 27 be 8f 4c 58 84 15 c7
                     68 3b ad 06 fa 79 25 44 63 b9 0e f0 47 a0 7f 26
    for i in 0 .. len-1:
        buf[i] ^= key[(i + 1) % 32]
        buf[i] ^= key[i % 32]

(= XOR with a 32-byte period stream 92 dd e4 b7 4a 95 7e c7 ...). Checked byte for byte on both pairs
in that folder: Guard.txt <-> Guard.dat (4772 bytes) and Guard.txt.old <-> Guard.dat.old (4778 bytes).

Plain format (Guard.txt)
------------------------
MySQL XML dump of table "outenserver", CRLF line ends, tab indents. Rows:
  id 0    root,    Child = number of groups (1)
  id 1    group 1, Child = number of servers in it
  id 101, 102, ...   one row per server:
     ServerName  name the client sends at login (16 bytes max) - must be known to the AuthServer
                 (AuthConfig.ini ServerN name or one of the ClientNames aliases)
     ServerIP / ServerPort   the LOGIN (account) server, e.g. 26.31.69.47 : 9961 - not the game port
     FlashName   name shown in the list
     FlashIcon   icon (Group3_server12), FlashHint (Hazem_Shield), Child 0, the other fields 0
The launcher writes this file itself from the patch server's /servers before the game starts
(Launcher\launcher.ini ServerListFile=Guard.dat).
