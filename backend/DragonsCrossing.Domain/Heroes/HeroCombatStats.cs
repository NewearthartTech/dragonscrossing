using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// These stats are used by combat, not necessarily owned by combat. For example, all these stats exist outside combat.
    /// TODO: Consider renaming this class to HeroStats as well as renaming HeroCombatStatsCalculated, to HeroStatsCalculated.
    /// </summary>
    public class HeroCombatStats : ChangeTracking
    {
        public int Id { get; set; }
        public int HeroId { get; set; }

        /// <summary>
        /// The Strength, Agility, Wisdom, Charisma and Quickness the user has not assigned after leveling up
        /// </summary>
        public int UnusedStats { get; set; } = 0;

        /// <summary>
        /// The current HP the hero has
        /// TODO: Move this to HeroCombatStatsCalculated since it can be adjusted based on items and abilities.
        /// </summary>
        public int HitPoints { get; set; } = 0;

        /// <summary>
        /// Max HP
        /// This can only change when a hero levels up or down
        /// </summary>
        public int MaxHitPoints { get; set; }

        /// <summary>
        /// The current XP the hero has.
        /// See HeroLevel for max xp.
        /// </summary>
        public int ExperiencePoints { get; set; } = 0;

        /// <summary>
        /// The base agility. 
        /// This can only change when a hero levels up or down
        /// </summary>
        public int MaxAgility { get; set; }

        /// <summary>
        /// The base wisdom.         
        /// This can only change when a hero levels up or down
        /// </summary>
        public int MaxWisdom { get; set; }

        /// <summary>
        /// The base strength. 
        /// This can only increase, not decrease (unless a hero dies).
        /// This is only adjusted during hero level up.
        /// </summary>
        public int MaxStrength { get; set; }

        /// <summary>
        /// The base quickness. 
        /// This can only increase, not decrease (unless a hero dies).
        /// This is only adjusted during hero level up.
        /// </summary>
        public int MaxQuickness { get; set; }

        /// <summary>
        /// The base charisma. 
        /// This can only increase, not decrease (unless a hero dies).
        /// This is only adjusted during hero level up.
        /// </summary>
        public int MaxCharisma { get; set; }
    }
}
