using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer6609.Game.MsgTournaments
{
    /// <summary>
    /// Legacy "UnionWar" entry point. The old custom pole war on map 20061 is not part of official Conquer Online and
    /// was disabled; the official Kingdom War (Gold Brick settlement + Plunder War) lives in MsgKingdomWar
    /// (Game/MsgTournaments/KingdomWar/). This class only mirrors its state for older callers:
    /// Proces = Plunder War state, Winner = the reigning Kingdom (union).
    /// </summary>
    public class MsgUnionWar
    {
        public bool SendInvitation = false;
        public class info
        {
            public uint UnionID;
            public string Name;
            public uint Score;

            public int LeaderReward = 1;
        }

        public List<uint> RewardLeader = new List<uint>();
        public List<uint> RewardDeputiLeader = new List<uint>();

        public DateTime StampRound = new DateTime();
        public DateTime StampShuffleScore = new DateTime();

        ProcesType proces = ProcesType.Dead;
        /// <summary>Alive while the Plunder War runs.</summary>
        public ProcesType Proces
        {
            get { return MsgKingdomWar.WarState == ProcesType.Alive ? ProcesType.Alive : proces; }
            set { proces = value == ProcesType.Alive ? ProcesType.Dead : value; }
        }

        public Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures { get; set; }
        public ConcurrentDictionary<uint, info> ScoreList;
        /// <summary>The union crowned by the last Kingdom War settlement (set once per crowning), or null.</summary>
        public info Winner;
        public Role.GameMap Map;

        public MsgUnionWar()
        {
            Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
            ScoreList = new ConcurrentDictionary<uint, info>();
            Winner = null;
        }

        /// <summary>Kept for old callers: the Kingdom War initializes itself on the kernel thread.</summary>
        public void Create()
        {
        }

        internal void SyncWinner(Role.Instance.Union kingdom)
        {
            Winner = kingdom == null ? null : new info() { UnionID = kingdom.UID, Name = kingdom.Name, Score = kingdom.GoldBrick };
        }

        /// <summary>No pole on the official Kingdom War; kept so old hooks compile.</summary>
        internal void UpdateScore(Role.Player client, uint Damage)
        {
        }
    }
}
