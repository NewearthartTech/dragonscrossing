using DragonsCrossing.Domain.Monsters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class MonsterPersonalitySeed
    {
        public MonsterPersonalitySeed(ModelBuilder builder)
        {
            builder.Entity<MonsterAttribute>().HasData(
                new MonsterAttribute()
                {
                    Id = 1,
                    Type = MonsterAttributeType.Charisma,   
                    MonsterPersonalityId = 1,
                },
                new MonsterAttribute()
                {
                    Id = 2,
                    Type = MonsterAttributeType.Charisma,
                    MonsterPersonalityId = 2,
                }, new MonsterAttribute()
                {
                    Id = 3,
                    Type = MonsterAttributeType.Power,
                    MonsterPersonalityId = 3,
                },
                new MonsterAttribute()
                {
                    Id = 4,
                    Type = MonsterAttributeType.Power,
                    MonsterPersonalityId = 4,
                },
                new MonsterAttribute()
                {
                    Id = 5,
                    Type = MonsterAttributeType.Quickness,
                    MonsterPersonalityId = 5,
                },
                new MonsterAttribute()
                {
                    Id = 6,
                    Type = MonsterAttributeType.Quickness,
                    MonsterPersonalityId = 6,
                },
                new MonsterAttribute()
                {
                    Id = 7,
                    Type = MonsterAttributeType.SpecialAbilityChance,
                    MonsterPersonalityId = 7,
                },
                new MonsterAttribute()
                {
                    Id = 8,
                    Type = MonsterAttributeType.SpecialAbilityChance,
                    MonsterPersonalityId = 8,
                },
                new MonsterAttribute()
                {
                    Id = 9,
                    Type = MonsterAttributeType.CriticalHitChance,
                    MonsterPersonalityId = 9,
                },
                new MonsterAttribute()
                {
                    Id = 10,
                    Type = MonsterAttributeType.CriticalHitChance,
                    MonsterPersonalityId = 10,
                },
                new MonsterAttribute()
                {
                    Id = 11,
                    Type = MonsterAttributeType.Charisma,
                    MonsterPersonalityId = 11,
                },
                new MonsterAttribute()
                {
                    Id = 12,
                    Type = MonsterAttributeType.Power,
                    MonsterPersonalityId = 11,
                },
                new MonsterAttribute()
                {
                    Id = 13,
                    Type = MonsterAttributeType.Quickness,
                    MonsterPersonalityId = 11,
                },
                new MonsterAttribute()
                {
                    Id = 14,
                    Type = MonsterAttributeType.SpecialAbilityChance,
                    MonsterPersonalityId = 11,
                },
                new MonsterAttribute()
                {
                    Id = 15,
                    Type = MonsterAttributeType.CriticalHitChance,
                    MonsterPersonalityId = 11,
                });

            builder.Entity<MonsterPersonality>().HasData(
                new MonsterPersonality()
                {
                    Id = 1,
                    PersonalityType = MonsterPersonalityType.Sickly,
                    EffectChance = 7,
                    EffectAmount = -30,
                },
                new MonsterPersonality()
                {
                    Id = 2,
                    PersonalityType = MonsterPersonalityType.Inspired,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 3,
                    PersonalityType = MonsterPersonalityType.Fatigued,
                    EffectChance = 7,
                    EffectAmount = -30,
                },
                new MonsterPersonality()
                {
                    Id = 4,
                    PersonalityType = MonsterPersonalityType.Brutal,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 5,
                    PersonalityType = MonsterPersonalityType.Lazy,
                    EffectChance = 7,
                    EffectAmount = -30,
                },
                new MonsterPersonality()
                {
                    Id = 6,
                    PersonalityType = MonsterPersonalityType.Lean,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 7,
                    PersonalityType = MonsterPersonalityType.Stupid,
                    EffectChance = 7,
                    EffectAmount = -30,
                },
                new MonsterPersonality()
                {
                    Id = 8,
                    PersonalityType = MonsterPersonalityType.Arcane,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 9,
                    PersonalityType = MonsterPersonalityType.Impotent,
                    EffectChance = 7,
                    EffectAmount = -30,
                },
                new MonsterPersonality()
                {
                    Id = 10,
                    PersonalityType = MonsterPersonalityType.Reckless,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 11,
                    PersonalityType = MonsterPersonalityType.Deadly,
                    EffectChance = 7,
                    EffectAmount = 30,
                },
                new MonsterPersonality()
                {
                    Id = 12,
                    PersonalityType = MonsterPersonalityType.None,
                    EffectChance = 23,
                    EffectAmount = 0,
                });
        }
    }
}
