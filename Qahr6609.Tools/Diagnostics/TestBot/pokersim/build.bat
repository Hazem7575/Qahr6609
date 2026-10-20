@echo off
rem Builds PokerSim.exe from this folder + the server's poker engine sources (no server dependencies).
setlocal
cd /d "%~dp0"
"C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\Roslyn\csc.exe" /nologo /optimize+ /target:exe /out:PokerSim.exe PokerSim.cs PokerSimTour.cs ..\..\..\Qahr6609.GameServer\Game\MsgServer\Poker\PokerEngine.cs ..\..\..\Qahr6609.GameServer\Game\MsgServer\Poker\PokerHandEvaluator.cs ..\..\..\Qahr6609.GameServer\Game\MsgServer\Poker\PokerTournament.cs
exit /b %errorlevel%
