using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Heroes
{
    public class LevelUpGenerator
    {
        private readonly Hero hero;

        public LevelUpGenerator(Hero hero)
        {
            this.hero = hero;
        }
        /// <summary>
        /// stats and other things are reduced based on current level.
        /// NOTE: If the hero levels up don't set tile visibility since they still need to discover the tile.
        /// </summary>
        /// <param name="hero"></param>
        public void LevelDownHero()
        {
            // TODO: load snapshot of previous level
            // TODO: look at table ref doc for details on how many levels to drop to
            hero.Level.Number -= 1; 
            if (hero.Level.Number <= 0)
                hero.Level.Number = 1;
            hero.CombatStats.ExperiencePoints = 0;
            // TODO: set tile visibility based on new level.
            // TODO: do we need to remove discovered tiles?
        }

        public void LevelUpHero (HeroLevel nextLevel)
        {
            var random = new Random();
            hero.Level = nextLevel;
            hero.CombatStats.MaxHitPoints += 5 + random.Next(1, 11); // 1D10 dice
            hero.UnusedSkillPoints += 2;
            //hero.CombatStats.ExperiencePoints = 0;
            hero.CombatStats.UnusedStats += 5 + random.Next(1, 5); // 1D4 dice
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experiencePoints"></param>
        public void AdjustExperience(int experiencePoints)
        {
            hero.CombatStats.ExperiencePoints += experiencePoints;
        }

        /// <summary>
        /// If hero exp >= max exp then level up hero and reset current exp. Set max exp according to level.
        /// </summary>
        /// <returns></returns>
        internal bool ShouldLevelUpHero()
        {
            return hero.CombatStats.ExperiencePoints >= hero.Level.MaxExperiencePoints;
        }
    }
}
