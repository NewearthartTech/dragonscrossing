using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    /// <summary>
    /// What the special ability does
    /// </summary>
    public class MonsterSpecialAbilityEffect
    {
        public int Id { get; set; }
        public int MonsterSpecialAbilityId { get; set; }
        /// <summary>
        /// The type of effect
        /// </summary>
        public MonsterSpecialAbilityEffectType Type { get; set; }

        public MonsterSpecialAbilityEffectWho EffectWho { get; set; }
        /// <summary>
        /// The amount the effect does. Could be a positive or negative number. 
        /// For some effects it can represent a percentage, for others its a total. Ex:
        /// 1. Parry: percent amount
        /// 2. MultipleAttack: number of attacks
        /// 3. Flee: 0 = no flee, 1 = yes flee        
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// For effects that last the entire battle this should be set to int.Max or some very large number.
        /// A single round is a monster + players turn
        /// This will be used in combat to determine if the effect is active in the current turn.
        /// </summary>
        public int TurnDuration { get; set; }     
    }
}
