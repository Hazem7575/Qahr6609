using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer6609.Game.MsgServer;
using Conquer6609.Game.MsgFloorItem;
using Conquer6609.MsgInterServer.Packets;
using Conquer6609.Game.MsgNpc;
using Conquer6609.Game.MsgTournaments;
using Conquer6609.Database;

namespace Conquer6609.Game.MsgMonster
{
    public unsafe partial class MonsterRole
    {
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int SecoundStamp = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Secounds, RemoveOnDead, SecoundStamp);
        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag, Role.GameMap map)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }
        public void ClearFlags(bool SendScreem = false)
        {
            BitVector.GetClear();
            UpdateFlagOffset(SendScreem);
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        private unsafe void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }
    }
}
