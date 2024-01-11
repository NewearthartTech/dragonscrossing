using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    /// <summary>
    /// The dynamically generated monster after all calculations. This needs to be stored in a table so 
    /// it can be retrieved each round by id. We don't want someone modifying attributes in the http request.
    /// This will help with the game state as well.
    /// </summary>
    public class Monster : ChangeTracking
    {
        public int Id { get; set; }
        /// <summary>
        /// The template this monster was based on. Ie: The base monster
        /// </summary>
        public MonsterTemplateOld MonsterTemplateOld { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public double Power { get; set; }
        public double Charisma { get; set; }
        /// <summary>
        /// Used to calculate LevelScaling value (see Game Mechanics)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// A percentage that determines how likely the monster is to avoid any damage
        /// aka: Difficulty To Hit(DTH)
        /// </summary>
        public int DodgeRate { get; set; }
        /// <summary>
        /// A percentage that determines how likely the monster is to hit the hero
        /// aka: chance to hit (CTH)
        /// </summary>
        public int HitRate { get; set; }

        public double Quickness { get; set; }

        // Damage is calculated every turn, not one time when the monster is generated
        //public int Damage { get; set; }
        /// <summary>
        /// As a percent
        /// </summary>
        public double CriticalHitChance { get; set; }

        /// <summary>
        /// As a percent
        /// </summary>
        public double ParryChance { get; set; }
        /// <summary>
        /// The calculated Mitigation for melee damage
        /// </summary>
        public double MitigationMelee { get; set; }

        public double MitigationRange { get; set; }

        public double MitigationMagic { get; set; }

        public double SpecialAbilityCastChance { get; set; }

        /// <summary>
        /// The personality this monster has if any 
        /// </summary>
        public MonsterPersonality Personality { get; set; }

        /// <summary>
        /// The special abilities that were cast during combat.
        /// TODO: I could move this property inside the Combat class to be consistent where the HeroSkillsCasted are. However, either way works because a monster only exists inside combat.
        /// </summary>
        public List<MonsterSpecialAbilityCasted>? SpecialAbilitiesCasted { get; set; }
    }    
}
