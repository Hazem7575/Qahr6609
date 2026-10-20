using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Role.Instance
{
    public class PerfectionEffect
    {
        public int ToxinEraser, LightOfStamina, DivineGuard, CalmWind, LuckyStrike, StrikeLock, FreeSoul, StraightLife, DrainingTouch, BloodSpawn, CoreStrike, KillingFlash, MirrorOfSin, InvisibleArrow, ShieldBreak, AbsoluteLuck;
        public int PhysicalAttack, PhysicalDefence, MagicAttack, MagicDefense;

        public void Update(int perfectionLevel)
        {
            ToxinEraser = LightOfStamina = DivineGuard = CalmWind = LuckyStrike = StrikeLock = FreeSoul = StraightLife = DrainingTouch = BloodSpawn = CoreStrike = KillingFlash = MirrorOfSin = InvisibleArrow = ShieldBreak = AbsoluteLuck = 0;
            PhysicalAttack = PhysicalDefence = MagicAttack = MagicDefense = 0;

            if (perfectionLevel > 0) { ToxinEraser = 30; LightOfStamina = 5; DivineGuard = 5; }
            if (perfectionLevel > 2) CalmWind = 1;
            if (perfectionLevel > 9) { PhysicalAttack += 100; MagicAttack += 100; }
            if (perfectionLevel > 14) LuckyStrike = 1;
            if (perfectionLevel > 19) StrikeLock = 1;
            if (perfectionLevel > 24) FreeSoul = 1;
            if (perfectionLevel > 29) StraightLife = 1;
            if (perfectionLevel > 34) DrainingTouch = 1;
            if (perfectionLevel > 39) BloodSpawn = 1;
            if (perfectionLevel > 44) ToxinEraser = 35;
            if (perfectionLevel > 49) CoreStrike = 1;
            if (perfectionLevel > 54) KillingFlash = 1;
            if (perfectionLevel > 59) MirrorOfSin = 1;
            if (perfectionLevel > 64) InvisibleArrow = 1;
            if (perfectionLevel > 69) { PhysicalDefence += 100; MagicDefense += 100; }
            if (perfectionLevel > 74) { PhysicalAttack += 200; MagicAttack += 300; }
            if (perfectionLevel > 79) CoreStrike = 2;
            if (perfectionLevel > 84) ToxinEraser = 40;
            if (perfectionLevel > 89) CalmWind = 2;
            if (perfectionLevel > 94) FreeSoul = 2;
            if (perfectionLevel > 99) { PhysicalDefence += 200; MagicDefense += 50; }
            if (perfectionLevel > 104) StrikeLock = 2;
            if (perfectionLevel > 109) ToxinEraser = 45;
            if (perfectionLevel > 114) LuckyStrike = 2;
            if (perfectionLevel > 119) { PhysicalDefence += 200; MagicDefense += 100; }
            if (perfectionLevel > 124) StraightLife = 2;
            if (perfectionLevel > 129) FreeSoul = 3;
            if (perfectionLevel > 134) CoreStrike = 3;
            if (perfectionLevel > 139) { PhysicalAttack += 200; MagicAttack += 400; }
            if (perfectionLevel > 144) ToxinEraser = 50;
            if (perfectionLevel > 149) { PhysicalAttack += 300; MagicAttack += 500; }
            if (perfectionLevel > 154) ShieldBreak = 1;
            if (perfectionLevel > 159) { PhysicalDefence += 300; MagicDefense += 125; }
            if (perfectionLevel > 164) KillingFlash = 2;
            if (perfectionLevel > 169) CalmWind = 3;
            if (perfectionLevel > 174) StrikeLock = 3;
            if (perfectionLevel > 179) { PhysicalDefence += 400; MagicDefense += 125; }
            if (perfectionLevel > 184) ToxinEraser = 60;
            if (perfectionLevel > 189) MirrorOfSin = 2;
            if (perfectionLevel > 194) FreeSoul = 4;
            if (perfectionLevel > 199) ToxinEraser = 70;
            if (perfectionLevel > 204) CoreStrike = 4;
            if (perfectionLevel > 209) StraightLife = 3;
            if (perfectionLevel > 214) LuckyStrike = 3;
            if (perfectionLevel > 219) DrainingTouch = 2;
            if (perfectionLevel > 224) InvisibleArrow = 2;
            if (perfectionLevel > 229) ToxinEraser = 80;
            if (perfectionLevel > 234) { PhysicalAttack += 400; MagicAttack += 500; }
            if (perfectionLevel > 239) BloodSpawn = 2;
            if (perfectionLevel > 244) CoreStrike = 5;
            if (perfectionLevel > 249) CalmWind = 4;
            if (perfectionLevel > 254) ShieldBreak = 2;
            if (perfectionLevel > 259) ToxinEraser = 90;
            if (perfectionLevel > 264) StraightLife = 4;
            if (perfectionLevel > 269) { PhysicalDefence += 400; MagicDefense += 125; }
            if (perfectionLevel > 272) { PhysicalAttack += 400; MagicAttack += 500; }
            if (perfectionLevel > 275) DrainingTouch = 3;
            if (perfectionLevel > 278) FreeSoul = 5;
            if (perfectionLevel > 281) InvisibleArrow = 3;
            if (perfectionLevel > 284) CalmWind = 5;
            if (perfectionLevel > 287) BloodSpawn = 3;
            if (perfectionLevel > 290) MirrorOfSin = 3;
            if (perfectionLevel > 293) KillingFlash = 3;
            if (perfectionLevel > 296) ShieldBreak = 3;
            if (perfectionLevel > 299) ToxinEraser = 100;
            if (perfectionLevel > 302) StraightLife = 5;
            if (perfectionLevel > 305) { PhysicalAttack += 400; MagicAttack += 500; }
            if (perfectionLevel > 307) StrikeLock = 4;
            if (perfectionLevel > 309) { PhysicalDefence += 400; MagicDefense += 125; }
            if (perfectionLevel > 311) ShieldBreak = 4;
            if (perfectionLevel > 313) LuckyStrike = 4;
            if (perfectionLevel > 315) { PhysicalDefence += 500; MagicDefense += 125; }
            if (perfectionLevel > 317) { PhysicalAttack += 500; MagicAttack += 500; }
            if (perfectionLevel > 319) LuckyStrike = 5;
            if (perfectionLevel > 320) StrikeLock = 5;
            if (perfectionLevel > 321) ShieldBreak = 5;
            if (perfectionLevel > 322) { PhysicalDefence += 500; MagicDefense += 125; }
            if (perfectionLevel > 323) { PhysicalAttack += 500; MagicAttack += 500; }
            if (perfectionLevel > 324) AbsoluteLuck = 5;
        }
    }
}
