using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    public enum MonsterSpecialAbilityEffectType
    {
        MultipleAttack,
        /// <summary>
        /// Damage To Player (DTP)
        /// </summary>
        DamageRate,
        /// <summary>
        /// 
        /// </summary>
        DodgeRate,
        Escape,
        /// <summary>
        /// 
        /// </summary>
        CriticalHitRate,
        //Dodge,
        Parry,
        Armor,
    }
}
