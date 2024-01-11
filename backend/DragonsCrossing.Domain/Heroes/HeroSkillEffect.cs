using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    public class HeroSkillEffect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public HeroSkillEffectType Type { get; set; }
        public HeroSkillEffectWho EffectWho { get; set; }
        /// <summary>
        /// The amount the effect does. Could be a positive or negative number depending on who the skill effects. 
        /// For some effects it can represent a percentage, for others its a total. Ex:
        /// 1. Parry: percent amount
        /// 2. MultipleAttack: number of attacks
        /// 3. Flee: 0 = no flee, 1 = yes flee        
        /// </summary>
        public int Amount { get; set; }
        public int TurnDuration { get; set; }
    }
}
