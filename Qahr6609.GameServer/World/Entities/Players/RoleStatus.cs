using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Role.Instance
{
    public class RoleStatus
    {
        public enum StatuTyp : byte
        {
            IncreaseMStrike = 60,
            IncreasePStrike = 59,
            IncreaseImunity = 61,
            IncreaseBreack = 62,
            IncreaseAntiBreack = 63,
            IncreaseMaxHp = 64,
            IncreasePAttack = 65,
            IncreaseMAttack = 66,
            IncreaseFinalPDamage = 67,
            IncreaseFinalMDamage = 68,
            IncreaseFinalPAttack = 69,
            IncreaseFinalMAttack = 70,

        }

        private uint Power = 0;

        public Extensions.Time32 Stamp = new Extensions.Time32();
        public RoleStatus(uint _Power, int Secounds)
        {
            Power = _Power;
            Stamp = Extensions.Time32.Now.AddSeconds(Secounds);
        }

        public uint GetPower
        {
            get { return Power; } 
        }
        public static implicit operator bool(RoleStatus big)
        {
            if (Extensions.Time32.Now > big.Stamp)
                return true;
            return false;
        }
        public static implicit operator uint(RoleStatus big)
        {
            return big.GetPower;
        }
    }
}
