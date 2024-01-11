using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Heroes
{
    /// <summary>
    /// Creates a new hero based on criteria from the db
    /// </summary>
    public class HeroGenerator
    {
        private HeroTemplate heroTemplate;
        private readonly List<HeroName> heroNames;

        public HeroGenerator(HeroTemplate heroTemplate, List<HeroName> heroNames)
        {
            this.heroTemplate = heroTemplate;
            this.heroNames = heroNames;
        }

        public Hero CreateHero(int generation, HeroLevel heroLevel)
        {
            var gender = RandomEnum<HeroGender>.Rand();
            var heroClass = RandomEnum<CharacterClass>.Rand();
            var newHero = new Hero()
            {
                HeroTemplate = heroTemplate,
                PlayerId = 1, // TODO: get this from authentication or set it to the admin player (that could be risky because the admin player will own all of the heroes).
                Generation = generation,
                HeroClass = heroClass,
                Gender = gender,
                Level = heroLevel,
                HeroName = heroNames.Rand(),
                Rarity = GetHeroRarity(),
                UnusedSkillPoints = 7,
                ImageBaseUrl = heroTemplate.ImageBaseUrl +
                    gender.ToString() + "-" + 
                    heroClass.ToString() + "-" + 
                    "gen" + generation,
                CreatedBy = "dcx-admin",
                DateCreated = DateTime.Now,                
            };

            newHero.CombatStats = new HeroCombatStats()
            {
                HitPoints = heroTemplate.MaxHitPoints,
                MaxHitPoints = heroTemplate.MaxHitPoints,
                ExperiencePoints = 0,
                UnusedStats = 0,
                MaxAgility = heroTemplate.StartingStatPointsEachStat.GetRandom() + (newHero.HeroClass == CharacterClass.Ranger ? 4 : 0),
                MaxWisdom = heroTemplate.StartingStatPointsEachStat.GetRandom() + (newHero.HeroClass == CharacterClass.Mage ? 4 : 0),
                MaxStrength = heroTemplate.StartingStatPointsEachStat.GetRandom() + (newHero.HeroClass == CharacterClass.Warrior ? 4 : 0),
                MaxQuickness = heroTemplate.StartingStatPointsEachStat.GetRandom(),
                MaxCharisma = heroTemplate.StartingStatPointsEachStat.GetRandom(),
            };
            return newHero;
        }    
        
        private HeroRarity GetHeroRarity()
        {
            var randomNum = new RangeDouble(0, 100).GetRandom();
            double total = 3.4;
            if (randomNum < total)
                return HeroRarity.Mythic;
            total += 6.4;
            if (randomNum < total)
                return HeroRarity.Legendary;
            total += 12.9;
            if (randomNum < total)
                return HeroRarity.Rare;
            total += 25.8;
            if (randomNum < total)
                return HeroRarity.Uncommon;            
            return HeroRarity.Common; 
        }        
    }
}
