using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// the player must go back to town and see the adventuring guild to level up. Leveling up also requires paying a fee in DCX. 
    /// The bonus of leveling up is as follows:
    /// HP: 5 + 1D10
    /// Skill points: +2
    /// All other attributes: the player will spend 5 + 1D4 across any attributes they feel relevant
    /// </summary>
    public class HeroLevel
    {
        public HeroLevel()
        {
        }

        public HeroLevel(int level)
        {
            Number = level;
        }

        public int Id { get; set; }

        /// <summary>
        /// Level 1, 2, 3, etc...
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The max experience points for this level
        /// </summary>
        public int MaxExperiencePoints { get; set; }

        //public int MaxHitPoints { get; set; }

        /// <summary>
        /// The number of quests that increase the max quests for this level of hero
        /// </summary>
        public int AdditionalQuests { get; set; }
    }
}
