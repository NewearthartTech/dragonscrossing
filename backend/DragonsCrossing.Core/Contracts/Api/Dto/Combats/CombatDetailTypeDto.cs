using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public enum CombatDetailTypeDto
    {
        StartTurn = 0,
        /// <summary>        
        /// Whoever goes first strikes first and can kill the other before the others turn.
        /// </summary>
        DoesHeroGoFirst = 1,
        MonsterSpecialAbility = 2,
        TryToHit = 3,
        OpponentDodge = 4,
        CalculateBaseDamage = 5,
        OpponentParry = 6,
        CriticalHit = 7,
        CalculateFinalDamage = 8,
        EndTurn = 9,
    }
}
