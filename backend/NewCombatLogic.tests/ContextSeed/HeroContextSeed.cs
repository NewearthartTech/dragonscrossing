using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.NewCombatLogic.tests.ContextSeed
{
    public class HeroContextSeed
    {
        public static HeroDto HeroSeedData
        {
            get
            {
                return new HeroDto
                {
                    id = 1,
                    playerId ="1",
                    name = "Legolas",
                    heroClass = CharacterClassDto.Warrior,
                    gender = Gender.Male,
                    generation = 0,
                    rarity = HeroRarityDto.Legendary,
                    remainingHitPointsPercentage = 100.0,
                    
                    isAscended = false,
                    prestigeLevel = 0,
                    level = 2,
                    experiencePoints = 3,
                    maxExperiencePoints = 20,
                    remainingQuests = 5,
                    maxDailyQuests = 10,
                    skills = null, // Still need to be populated
                    isHearthstoneAvailable = true,
                    baseChanceToHit = 20,
                    learnedSkillCapacity = 3,
                    zoneBackgroundType = ZoneBackgroundTypeDto.Swamp,
                    difficultyToHit = 15,
                    unsecuredDCXValue = new UnsecurdDcx { amount = 5},
                    inventory = null, // Still need to be populated
                    baseSkillPoints = 5,
                    strength = 5,
                    agility = 3,
                    wisdom = 2,
                    quickness = 3,
                    charisma = 4,          
                };
            }
        }
    }
}
