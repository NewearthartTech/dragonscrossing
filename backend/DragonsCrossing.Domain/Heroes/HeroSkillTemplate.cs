using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    public class HeroSkillTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Each skill belongs to a certain hero class
        /// </summary>
        public CharacterClass HeroClass { get; set; }

        public List<HeroSkillEffect> Effects { get; set; }

        /// <summary>
        /// The skills the hero starts with. 
        /// Also, these skills can never be found or show up in loot.
        /// </summary>
        public bool IsStartingSkill { get; set; }

        /// <summary>
        /// The zones this skill is found in
        /// </summary>
        public List<HeroSkillTemplateZone>? Zones { get; set; }
    }
}
