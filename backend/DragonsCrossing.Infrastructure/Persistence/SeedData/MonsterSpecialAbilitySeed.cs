using DragonsCrossing.Domain.Monsters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class MonsterSpecialAbilitySeed
    {
        public MonsterSpecialAbilitySeed(ModelBuilder builder)
        {
            // wandering goblin
            builder.Entity<MonsterSpecialAbility>().HasData(
                new MonsterSpecialAbility()
                {
                    Id = 1,
                    MonsterTemplateId = 1,
                    Name = "Whirly Gig",
                    CanCastAgain = true,
                    Description = "The goblin rotates furiously, attacking with both daggers in a flurry of condensed rage.",
                    IsGuaranteedDamage = false,
                    UsesTurn = true,                        
                });
            builder.Entity<MonsterSpecialAbilityEffect>().HasData(
                new MonsterSpecialAbilityEffect()
                {
                    Id = 1,                    
                    MonsterSpecialAbilityId = 1,
                    Type = MonsterSpecialAbilityEffectType.DamageRate,
                    Amount = 120,
                    TurnDuration = 1,
                    EffectWho = MonsterSpecialAbilityEffectWho.Monster
                });

            // Rival Adventurer
            builder.Entity<MonsterSpecialAbility>().HasData(
                new MonsterSpecialAbility()
                {
                    Id = 2,
                    MonsterTemplateId = 2,
                    Name = "Runic Axe",
                    CanCastAgain = true,
                    Description = "The runes on his axe glow distractingly, and you find it harder to focus on your own weapon work.",
                    IsGuaranteedDamage = false,
                    UsesTurn = false,
                });
            builder.Entity<MonsterSpecialAbilityEffect>().HasData(
                new MonsterSpecialAbilityEffect()
                {
                    Id = 2,
                    MonsterSpecialAbilityId = 2,
                    Type = MonsterSpecialAbilityEffectType.Parry,
                    EffectWho = MonsterSpecialAbilityEffectWho.Hero,
                    Amount = -50,
                    TurnDuration = 2,
                });

            // Giant Wolf
            builder.Entity<MonsterSpecialAbility>().HasData(
                new MonsterSpecialAbility()
                {
                    Id = 3,
                    MonsterTemplateId = 3,
                    Name = "Savage Bite",
                    CanCastAgain = true,
                    Description = "Slavering, the wolf bites you with its foaming maw.",
                    IsGuaranteedDamage = false,
                    UsesTurn = true,
                });
            builder.Entity<MonsterSpecialAbilityEffect>().HasData(
                new MonsterSpecialAbilityEffect()
                {
                    Id = 3,
                    MonsterSpecialAbilityId = 3,
                    Type = MonsterSpecialAbilityEffectType.DamageRate,
                    EffectWho = MonsterSpecialAbilityEffectWho.Monster,
                    Amount = 140,
                    TurnDuration = 1,
                });

            // Mighty Stag
            builder.Entity<MonsterSpecialAbility>().HasData(
                new MonsterSpecialAbility()
                {
                    Id = 4,
                    MonsterTemplateId = 4,
                    Name = "Majesty",
                    CanCastAgain = false,
                    Description = "The countenance of this lord of the forest is staggering. His sheer majesty pushes you away.",
                    IsGuaranteedDamage = false,
                    UsesTurn = false,
                });
            builder.Entity<MonsterSpecialAbilityEffect>().HasData(
                new MonsterSpecialAbilityEffect()
                {
                    Id = 4,
                    MonsterSpecialAbilityId = 4,
                    Type = MonsterSpecialAbilityEffectType.DodgeRate,
                    EffectWho = MonsterSpecialAbilityEffectWho.Monster,
                    Amount = 10,
                    TurnDuration = 1,
                });
        }
    }
}
