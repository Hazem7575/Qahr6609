// NPC ids of the event (were NpcID.Ramadan* in the server). A module brings its own ids: a cast to NpcID works
// in [NpcAttribute] like an NpcID member. Keep them equal to the uids in data\event.ini [Npcs].
namespace Conquer6609.Game.Ramadan
{
    public static class RamadanNpcIds
    {
        public const Game.MsgNpc.NpcID
            RamadanGuardian = (Game.MsgNpc.NpcID)8155001, RamadanLamplighter = (Game.MsgNpc.NpcID)8155002,
            RamadanScorchedLantern = (Game.MsgNpc.NpcID)8155003, RamadanCaptainRashid = (Game.MsgNpc.NpcID)8155004,
            RamadanSageNoor = (Game.MsgNpc.NpcID)8155005, RamadanMasterKareem = (Game.MsgNpc.NpcID)8155006,
            RamadanWidowSalma = (Game.MsgNpc.NpcID)8155007, RamadanNightWatchman = (Game.MsgNpc.NpcID)8155008,
            RamadanCaravanMaster = (Game.MsgNpc.NpcID)8155009, RamadanGateKeeper = (Game.MsgNpc.NpcID)8155010,
            RamadanCrescentMerchant = (Game.MsgNpc.NpcID)8155011, RamadanCrescentHerald = (Game.MsgNpc.NpcID)8155012;
    }
}
