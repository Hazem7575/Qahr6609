// QuestEngine module: runs the official Conquer Online quests (client Quest List) that have no scripted code
// on this server. Engine: QuestEngine.cs. Data: <DbLocation>\Modules\QuestEngine\quests.ini + quests_*.ini
// (format: docs\QUEST_FORMAT.md). Load warnings go to load_report.txt in the same folder.
// Switch off: Qahr6609.Tools\Server\ServerCtl\module_off.bat QuestEngine + rebuild (players keep their quest progress).
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.QuestEngine
{
    public sealed class QuestEngineModule : ModuleBase
    {
        public override string Name { get { return "QuestEngine"; } }
        public override string Version { get { return "2.0"; } }

        public override void OnLoad()
        {
            QuestEngine.DataDir = DataPath;
            QuestEngine.LogLine = Log;
            QuestEngine.Init();
        }

        public override void OnTick(Extensions.Time32 clock) { QuestEngine.Tick(); }
        public override void OnLogout(Client.GameClient client) { QuestEngine.OnLogout(client); }
        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster) { QuestEngine.OnKill(killer, monster); }
        public override void OnPlayerKilled(Role.Player killer, Role.Player victim) { QuestEngine.OnPlayerKilled(killer, victim); }
        public override bool OnItemUse(Client.GameClient client, Game.MsgServer.MsgGameItem item) { return QuestEngine.OnItemUse(client, item); }
        public override int OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte option, uint npcId) { return QuestEngine.OnNpc(client, stream, option, npcId); }

        [GmCommand("quest", Role = CommandRole.GM, Help = "generic quests: @quest | info <id> | npc <npcId> | accept <id> | done <id> | finish <id> | reset <id> | reload | check", Usage = "@quest accept 3482")]
        static void QuestCommand(Client.GameClient client, string[] args)
        {
            QuestEngine.GmCommand(client, args);
        }
    }
}
