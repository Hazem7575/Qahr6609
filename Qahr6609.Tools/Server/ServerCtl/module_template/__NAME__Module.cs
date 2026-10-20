// __NAME__ module - created by Qahr6609.Tools\Server\ServerCtl\new_module.bat from Qahr6609.Tools\Server\ServerCtl\module_template.
// Delete what you do not need. Read docs\MODULES.md for every hook and the rules.
//
// Active = this folder exists in Qahr6609.GameServer\Modules\__NAME__\ . Disable: Qahr6609.Tools\Server\ServerCtl\module_off.bat __NAME__ , then rebuild.
// Settings: shell.ini section [Module.__NAME__]   Data + logs: <DbLocation>\Modules\__NAME__\
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using Conquer6609.Framework.Commands;
using Conquer6609.Framework.Modules;

namespace Conquer6609.Modules.__NAME__
{
    public sealed class __NAME__Module : ModuleBase
    {
        public override string Name { get { return "__NAME__"; } }
        public override string Version { get { return "1.0"; } }
        public override int Priority { get { return 0; } }   // lower runs first

        /// <summary>Set at the end of OnLoad so static handlers (NPC, commands) can reach the module.</summary>
        internal static __NAME__Module Instance;

        /// <summary>NPC UID handled by this module (also the UID used when the NPC is spawned below).</summary>
        public const uint NpcUid = __NPCID__;

        // settings (shell.ini [Module.__NAME__]); defaults keep the module silent
        string greeting = "";
        uint npcMap;
        ushort npcX, npcY, npcLook;

        // shared state: several game threads call the hooks -> concurrent collections only
        readonly ConcurrentDictionary<uint, int> sessionKills = new ConcurrentDictionary<uint, int>();
        long totalKills;

        // ------------------------------------------------------------------ lifecycle
        public override void OnLoad()
        {
            greeting = ConfigString("Greeting", "");
            npcMap = (uint)ConfigInt("NpcMap", 0);          // 0 = do not spawn the NPC
            npcX = (ushort)ConfigInt("NpcX", 0);
            npcY = (ushort)ConfigInt("NpcY", 0);
            npcLook = (ushort)ConfigInt("NpcLook", 0);       // mesh = look * 10 + direction

            // load saved data (OnLoad runs once at startup, file I/O is fine here)
            string file = DataPath + "stats.txt";
            long saved;
            if (File.Exists(file) && long.TryParse(File.ReadAllText(file).Trim(), out saved))
                totalKills = saved;

            if (npcMap != 0) SpawnNpc();
            Log("loaded, totalKills=" + totalKills);
            Instance = this;   // last: the NPC / command handlers only see a fully loaded module
        }

        public override void OnSave()
        {
            // background save thread: short file writes are OK here (not in the other hooks)
            File.WriteAllText(DataPath + "stats.txt", System.Threading.Interlocked.Read(ref totalKills).ToString());
        }

        public override void OnShutdown()
        {
            OnSave();
            Log("shutdown");
        }

        // ------------------------------------------------------------------ player
        public override void OnLogin(Client.GameClient client)
        {
            if (greeting.Length > 0)
                client.SendSysMesage(greeting.Replace("{name}", client.Player.Name));
        }

        public override void OnLogout(Client.GameClient client)
        {
            int dummy;
            sessionKills.TryRemove(client.Player.UID, out dummy);
        }

        // ------------------------------------------------------------------ combat
        public override void OnMonsterKilled(Client.GameClient killer, Game.MsgMonster.MonsterRole monster)
        {
            if (killer == null || killer.Player == null) return;
            sessionKills.AddOrUpdate(killer.Player.UID, 1, (uid, n) => n + 1);
            System.Threading.Interlocked.Increment(ref totalKills);
        }

        // Other hooks you can override (see docs\MODULES.md):
        //   public override void OnTick(Extensions.Time32 clock) { }                         // ~1 s
        //   public override bool OnMove(Client.GameClient c, ushort x, ushort y) { return true; } // false = block
        //   public override uint OnMonsterDamage(Client.GameClient a, Game.MsgMonster.MonsterRole m, uint dmg) { return dmg; }
        //   public override void OnPlayerKilled(Role.Player killer, Role.Player victim) { }  // killer may be null
        //   public override bool OnItemUse(Client.GameClient c, Game.MsgServer.MsgGameItem item) { return false; } // true = handled

        // ------------------------------------------------------------------ NPC
        void SpawnNpc()
        {
            Role.GameMap map;
            if (Database.Server.ServerMaps == null || !Database.Server.ServerMaps.TryGetValue(npcMap, out map))
            {
                Log("NPC not spawned: map " + npcMap + " not loaded");
                return;
            }
            if (map.View.MapContain(Role.MapObjectType.Npc, NpcUid))
            {
                Log("NPC not spawned: uid " + NpcUid + " already used on map " + npcMap);
                return;
            }
            var np = Game.MsgNpc.Npc.Create();
            np.UID = NpcUid;
            np.NpcType = Role.Flags.NpcType.Talker;
            np.Mesh = (ushort)(npcLook * 10 + 0);
            np.Map = npcMap;
            np.X = npcX;
            np.Y = npcY;
            map.AddNpc(np);
        }

        // NPC dialog handler: the id is a plain number cast to NpcID (no need to edit NpcID.cs).
        // Signature must be exactly: static void (Client.GameClient, ServerSockets.Packet, byte, string, uint).
        // Option 0 = first click; the other numbers are the ids you pass to AddOption. 255 = close.
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)NpcUid)]
        public static void OnNpcTalk(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var self = Instance;
            if (self == null) return;
            var dialog = new Game.MsgNpc.Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    dialog.AddText("Hello " + client.Player.Name + ". I belong to the __NAME__ module.");
                    dialog.AddOption("How many monsters did I kill?", 1);
                    dialog.AddOption("Bye.", 255);
                    dialog.AddAvatar(0).FinalizeDialog();
                    break;
                case 1:
                    int n;
                    self.sessionKills.TryGetValue(client.Player.UID, out n);
                    dialog.AddText("This session you killed " + n + " monster(s).");
                    dialog.AddOption("Thanks.", 255);
                    dialog.AddAvatar(0).FinalizeDialog();
                    break;
            }
        }

        // ------------------------------------------------------------------ chat command
        // args[0] = command word, args[1] = first argument ...  Role: Player / GM / ProjectManager.
        [GmCommand("__name__", Role = CommandRole.GM, Help = "__NAME__ module status", Usage = "@__name__ [reset]")]
        static void Command(Client.GameClient client, string[] args)
        {
            var self = Instance;
            if (self == null) return;
            if (args.Length > 1 && args[1] == "reset")
            {
                System.Threading.Interlocked.Exchange(ref self.totalKills, 0);
                self.Log(client.Player.Name + " reset the counter");
            }
            client.SendSysMesage("[__NAME__] total kills: " + System.Threading.Interlocked.Read(ref self.totalKills)
                + ", online players tracked: " + self.sessionKills.Count);
        }
    }
}
