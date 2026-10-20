Module template (used by Qahr6609.Tools\Server\ServerCtl\new_module.bat) - see docs\MODULES.md for the full guide.

Create a module:
    Qahr6609.Tools\Server\ServerCtl\new_module.bat DailyGift            (optional 2nd arg: NPC uid for the example NPC)
    -> Qahr6609.GameServer\Modules\DailyGift\DailyGiftModule.cs
    then rebuild (Qahr6609.Tools\Server\ServerCtl\step2_build.bat). The folder being there = the module is active.

Disable / enable:
    Qahr6609.Tools\Server\ServerCtl\module_off.bat DailyGift   -> zips to Qahr6609.GameServer\Modules\_off\DailyGift.zip and deletes the folder
    Qahr6609.Tools\Server\ServerCtl\module_on.bat  DailyGift   -> restores the newest zip (the zip is kept)
    rebuild after each.

Tokens replaced by new_module.bat in every file of this folder (README.txt is not copied):
    __NAME__   module name as typed          (DailyGift)
    __name__   module name in lower case     (dailygift)  -> used as the @command name
    __NPCID__  NPC uid for the example NPC   (8160000-8169999 picked automatically)

The example module shows:
    - OnLoad / OnSave / OnShutdown with a small data file in <DbLocation>\Modules\<Name>\
    - OnLogin / OnLogout / OnMonsterKilled with thread-safe state (ConcurrentDictionary, Interlocked)
    - an NPC dialog handler:   [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)NpcUid)]
                               static void X(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
    - optional NPC spawn from shell.ini [Module.<Name>] NpcMap / NpcX / NpcY / NpcLook (NpcMap=0 = no spawn)
    - a chat command:          [GmCommand("name", Role = CommandRole.GM, Help = "...")]
                               static void X(Client.GameClient client, string[] args)
    - Log("...") -> <DbLocation>\Modules\<Name>\logs\yyyyMMdd.log (async)

Do not put .cs files in Qahr6609.GameServer\Modules\_off (only zips live there).
