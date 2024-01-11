using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    /// <summary>
    /// A random personality given to a monster when it's generated
    /// </summary>
    public class MonsterPersonality
    {
        public int Id { get; set; }
        public string Name => PersonalityType.ToString();

        public MonsterPersonalityType PersonalityType { get; set; }

        /// <summary>
        /// How likely the monster will receive this personality
        /// aka: ApplyRate, GrantRate, 
        /// </summary>
        public double EffectChance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>EF doesn't support list of enum, so have to wrap it in a class</remarks>
        public List<MonsterAttribute> AttributesAffected { get; set; }
        /// <summary>
        /// The percentage amount to add/subtract from the affected attribute.
        /// ex: +30% or -20%
        /// </summary>
        public double EffectAmount { get; set; }
    }
}
