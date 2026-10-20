// Ramadan module ("The Legend of the Last Crescent"). The whole event is this folder; to give it to someone, give the folder.
//
//   Qahr6609.GameServer\Modules\Ramadan\      in the source: built into GameServer.exe by a normal build
//     RamadanModule.cs   this file: connects the event to the server (start, save, tick, login, kills, @ramadan)
//     code\              the event: story, nightly lanterns, caravan, boss, shop, the 12 NPC dialogs (RamadanNpcs.cs)
//     content\Monsters\  the 7 monster types (8155101-8155107), read at start-up (Modules\ModuleContent.cs)
//     data\              event.ini (dates, NPC places, rewards) + decor.txt, copied by the build to
//                        ServerDataBase\Modules\Ramadan\ when they are not there yet; players' progress is saved there too
//
// The same folder also works without a build: put it in ServerDataBase\Modules\Ramadan\ (the server compiles the
// .cs files at start-up). NPC ids are in code\RamadanNpcIds.cs, their places in data\event.ini [Npcs].
// Remove the event: delete the folder (or rename it to _Ramadan).
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;
using Conquer6609.Game.Ramadan;

namespace Conquer6609.Game.Ramadan.Module
{
    public sealed class RamadanModule : ModuleBase
    {
        public override string Name { get { return "Ramadan"; } }

        public override void OnLoad() { RamadanEvent.Init(); }
        public override void OnSave() { RamadanEvent.Save(); }
        public override void OnTick(Extensions.Time32 clock) { RamadanEvent.Tick(); }

        public override void OnLogin(Client.GameClient client) { RamadanEvent.OnLogin(client); }
        public override void OnLogout(Client.GameClient client) { RamadanEvent.OnLogout(client); }

        // mosque decor cells: the event pulls the player back itself
        public override bool OnMove(Client.GameClient client, ushort x, ushort y) { return !RamadanEvent.BlockMove(client, x, y); }

        public override uint OnMonsterDamage(Client.GameClient attacker, Game.MsgMonster.MonsterRole target, uint damage)
        {
            return RamadanEvent.AdjustMonsterDamage(attacker, target, damage);
        }

        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            RamadanEvent.OnMonsterKilled(killer, monster);
        }

        [GmCommand("ramadan", Role = CommandRole.GM, Help = "Ramadan event: @ramadan status | on | off | reload | caravan | decor ...")]
        static void RamadanCommand(Client.GameClient client, string[] args)
        {
            RamadanEvent.GmCommand(client, args);
        }
    }
}
