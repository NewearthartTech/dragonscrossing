using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Api.templates.monsters
{
    public static class MonsterContextSeedForEric
    {
        public static List<MonsterTemplate> MonsterTemplateSeed
        {
            get
            {
                return new List<MonsterTemplate>
                {
                    new MonsterTemplate
                    {
                        Name = "Wandering Goblin",
                        MonsterSlug = "Wandering Goblin",
                        Description = "Grumbling, stumbling, filthy and ragged, this little green creature's size does little to hide the wanton violence begging to burst at the first thing that looks at him the wrong way. For better or worse, today that happens to be you.",
                        MonsterItems = new string[] { "Dagger", "Knife" },
                        AppearChance = 10,
                        DieDamage = new DieDto[]{ new DieDto { Sides = 6 },  new DieDto { Sides = 6 } },
                        SpecialAbility = MonsterSpecialAbilities1,
                        MaxHitPoints = new Range { lower = 3, upper = 10 },
                        ChanceToHit = new Range { lower = 4, upper = 11 },
                        ChanceToDodge = new Range { lower = 5, upper = 12 },
                        DifficultyToHit = new Range { lower = 6, upper = 13 },
                        CritChance = new Range { lower = 7, upper = 14 },
                        ParryChance = new Range { lower = 8, upper = 15 },
                        Charisma = new Range { lower = 9, upper = 16 },
                        Quickness = new Range { lower = 10, upper = 17 },
                        Level = new Range { lower = 11, upper = 18 },
                        Power = new Range { lower = 12, upper = 19 },
                        ArmorMitigation = new Range { lower = 13, upper = 20 },
                        BonusDamage = new Range { lower = 14, upper = 21 },
                        ChanceOfUsingSpecialAbility = new Range { lower = 15, upper = 22 }
                    },
                    new MonsterTemplate
                    {
                        Name = "Rival Adventurer",
                        MonsterSlug = "Rival Adventurer",
                        Description = "Bad guy appears",
                        MonsterItems = new string[] { "Sword", "Shield" },
                        AppearChance = 10,
                        DieDamage = new DieDto[]{ new DieDto { Sides = 6 } },
                        SpecialAbility = MonsterSpecialAbilities2,
                        MaxHitPoints = new Range { lower = 3, upper = 10 },
                        ChanceToHit = new Range { lower = 4, upper = 11 },
                        ChanceToDodge = new Range { lower = 5, upper = 12 },
                        DifficultyToHit = new Range { lower = 6, upper = 13 },
                        CritChance = new Range { lower = 7, upper = 14 },
                        ParryChance = new Range { lower = 8, upper = 15 },
                        Charisma = new Range { lower = 9, upper = 16 },
                        Quickness = new Range { lower = 10, upper = 17 },
                        Level = new Range { lower = 11, upper = 18 },
                        Power = new Range { lower = 12, upper = 19 },
                        ArmorMitigation = new Range { lower = 13, upper = 20 },
                        BonusDamage = new Range { lower = 14, upper = 21 },
                        ChanceOfUsingSpecialAbility = new Range { lower = 15, upper = 22 }
                    }
                };
            }
        }

        public static MonsterSpecialAbilityDto MonsterSpecialAbilities1
        {
            get
            {
                return new MonsterSpecialAbilityDto()
                {
                    Id = 1,
                    Name = "Whirly Gig",
                    CanUseSpecialAbilityMoreThanOnce = false,
                    Affects = new StatusEffectDto[]
                    {
                        new StatusEffectDto
                        {
                            AffectType = CombatantType.Monster,
                            StatName = EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                            FriendlyStatName = "Dodge",
                            AffectAmount = 5,
                            Duration = 2
                        }
                    }
                };
            }
        }

        public static MonsterSpecialAbilityDto MonsterSpecialAbilities2
        {
            get
            {
                return new MonsterSpecialAbilityDto()
                {
                    Id = 2,
                    Name = "Torment",
                    CanUseSpecialAbilityMoreThanOnce = false,
                    Affects = new StatusEffectDto[]
                    {
                        new StatusEffectDto
                        {
                            AffectType = CombatantType.Monster,
                            StatName = EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                            FriendlyStatName = "Chance To Hit",
                            AffectAmount = 10,
                            Duration = 2
                        },
                        new StatusEffectDto
                        {
                            AffectType = CombatantType.Monster,
                            StatName = EffectedbySpecialAbilityStat.Q7_Crit,
                            FriendlyStatName = "Crit",
                            AffectAmount = 10,
                            Duration = 3
                        }
                    }
                };
            }
        }
    }
}