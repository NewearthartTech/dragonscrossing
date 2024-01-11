using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.NewCombatLogic.tests.ContextSeed
{
    public static class MonsterContextSeed
    {
        public static MonsterTemplate MonsterTemplateSeed
        {
            get
            {
                return new MonsterTemplate
                {
                    Name = "Wandering Goblin",
                    MonsterSlug = "Wandering Goblin",
                    Description = "Grumbling, stumbling, filthy and ragged, this little green creature's size does little to hide the wanton violence begging to burst at the first thing that looks at him the wrong way. For better or worse, today that happens to be you.",
                    MonsterItems = new string[] { "Dagger", "Knife" },
                    AppearChance = 10,
                    DieDamage = new DieDto[] { new DieDto() { Sides = 6}, new DieDto() { Sides = 6 } },
                    //LocationSlug = "1",
                    SpecialAbility = MonsterSpecialAbilities,
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
                };
            }
        }

        public static MonsterDto GenerateMonsterDto()
        {
            var dto = new MonsterDto();

            //reflection to avoid code duplication
            var monsterProps = typeof(MonsterDto)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(k => k.Name, v => v);

            var templateProps = typeof(MonsterTemplate).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in templateProps)
            {
                if (!monsterProps.ContainsKey(prop.Name))
                {
                    Debug.Assert(false, "We have template and DTO mismatch");
                    continue;
                }

                var dtoProp = monsterProps[prop.Name];
                if (!dtoProp.CanWrite)
                {
                    Debug.Assert(false, "We have template and DTO mismatch");
                    continue;
                }

                object value = prop.GetValue(MonsterTemplateSeed);

                if (null == value)
                    continue;

                if (prop.PropertyType == typeof(Range))
                {
                    var range = value as Range;

                    value = new Random().Next(range.lower, range.upper + 1);
                }

                dtoProp.SetValue(dto, value);
            }

            // The initial HitPoints is the same as MaxHitPoints
            dto.HitPoints = dto.MaxHitPoints;

            return dto;

        }

        public static MonsterSpecialAbilityDto MonsterSpecialAbilities
        {
            get
            {
                return new MonsterSpecialAbilityDto()
                {
                    Id = 1,
                    Name = "Runic Axe",
                    CanUseSpecialAbilityMoreThanOnce = false,
                    Affects = new StatusEffectDto[] 
                    { 
                        new StatusEffectDto 
                        { 
                            AffectType = CombatantType.Monster,
                            StatName = EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                            FriendlyStatName = "abc",
                            AffectAmount = 10,
                            Duration = 2
                        } 
                    }
                };
            }
        }
    }
}
