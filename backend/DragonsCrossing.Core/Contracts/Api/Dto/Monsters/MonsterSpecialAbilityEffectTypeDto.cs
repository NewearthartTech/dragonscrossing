using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Monsters
{
    public enum MonsterSpecialAbilityEffectTypeDto
    {
        MultipleAttack,
        /// <summary>
        /// Damage To Player (DTP)
        /// </summary>
        DamageRate,
        /// <summary>
        /// 
        /// </summary>
        DifficultyToHit,
        Escape,
        /// <summary>
        /// 
        /// </summary>
        CriticalHitRate,
        Dodge,
        Parry,
        Armor,
    }
}
