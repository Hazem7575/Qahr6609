using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer6609.Game.MsgTournaments
{
    public enum TournamentType : byte
    {  None = 0,
        DragonWar = 1,
        FootBall = 2,
        BattleField = 3,
        DBShower = 4,
        TeamDeathMatch = 5,
        LastManStand = 11,
        ExtremePk = 7,
        KillerOfElite = 8,
        TreasureThief =12, 
        FreezeWar = 10,
        SkillTournament = 6,
        KingOfTheHill = 9,
        TConquerWar = 14,// was 10, the same value as FreezeWar
        Count = 13,
        
        QuizShow = 15,

        DragonIsland = 16,
        MsgCpsArenaSingle = 17
    }
}
