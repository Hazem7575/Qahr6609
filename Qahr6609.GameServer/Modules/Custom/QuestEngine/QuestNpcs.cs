// Official quest NPCs that have no script of their own on this server: their talk goes straight to the quest engine.
// Every NPC click reaches the quest engine first through the core NPC hook (docs\COORDINATION.md R1); these handlers
// only give script-less quest NPCs a greeting when the engine has nothing to offer.
namespace Conquer6609.Modules.QuestEngine
{
    public static class QuestNpcs
    {
        public static readonly uint[] Ids = { 4293, 5674, 8301, 8303, 10019, 20001, 35015 };

        static void Talk(Client.GameClient c, ServerSockets.Packet s, byte option, uint id, string name)
        {
            // quest options were already offered by the core NPC hook (QuestEngine.OnNpc via ModuleManager.NpcTalk)
            if (option != 0) return;
            new Game.MsgNpc.Dialog(c, s).AddText("Greetings, traveler. I am " + name + ".").AddOption("Goodbye.", 255).AddAvatar(10).FinalizeDialog();
        }

        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)4293)]
        public static void MineAssistant(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "the Mine Assistant"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)5674)]
        public static void KungfuGirl(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "Kungfu Girl"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)8301)]
        public static void Quark(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "Quark"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)8303)]
        public static void Xavier(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "Xavier"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)10019)]
        public static void ArtisanWind(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "Artisan Wind"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)20001)]
        public static void SoldierLeader(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "the Soldier Leader"); }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)35015)]
        public static void BlessedGreatMaster(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id) { Talk(c, s, o, id, "the Blessed Great Master"); }

        // Twin City plaza (n-newplain): Guide on the dragon platform, LadyLuck at the north side of the square.
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)2610)]
        public static void TwinCityGuide(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id)
        {
            if (o != 0) return;
            new Game.MsgNpc.Dialog(c, s)
                .AddText("Welcome to Twin City! The shops, the warehouse and the Conductress are all around this square. ")
                .AddText("Open the Pathfinding window to walk straight to any NPC, and press Quest to see your missions.")
                .AddOption("Thank you.", 255).AddAvatar(10).FinalizeDialog();
        }
        [Game.MsgNpc.NpcAttribute((Game.MsgNpc.NpcID)9231)]
        public static void TwinCityLadyLuck(Client.GameClient c, ServerSockets.Packet s, byte o, string i, uint id)
        {
            Game.MsgNpc.NpcHandler.MarketLadyLuck(c, s, o, i, id);
        }
    }
}
