using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class MonsterTemplateSeed
    {
        public MonsterTemplateSeed(ModelBuilder builder)
        {            
            builder.Entity<MonsterTemplateOld>(b =>
            {
                b.HasData(new MonsterTemplateOld()
                {
                    Id = 1,
                    Name = "Wandering Goblin",
                    Description = "Grumbling, stumbling, filthy and ragged, this little green creature's size does little to hide the wanton violence begging to burst at the first thing that looks at him the wrong way. For better or worse, today that happens to be you.",
                    AppearanceChance = 20,
                    SpecialAbilityCastChance = 22,
                    ImageFilePath = @"\images\monsters\wandering_goblin.jpg",
                    MonsterClass = CharacterClass.Warrior,
                    WeaponDescription = "Dagger",
                    ArmorDescription = "Worn Rags and Leather",
                    CharismaOpportunityChance = 20,
                    CombatOpportunityChance = 10,
                    DcxLootChance = 10,
                    TileId = 1,
                });                
                b.OwnsOne(e => e.MitigationMelee).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 0d,
                        Max = 10d,
                    });
                b.OwnsOne(e => e.MitigationRange).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 10d,
                        Max = 20d,
                    });
                b.OwnsOne(e => e.MitigationMagic).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 0d,
                        Max = 10d,
                    });
                b.OwnsOne(e => e.MaxHitPoints).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 30,
                        Max = 35,
                    });
                b.OwnsOne(e => e.HitRate).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 0,
                        Max = 5,
                    });
                b.OwnsOne(e => e.Quickness).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 2,
                        Max = 12,
                    });
                b.OwnsOne(e => e.Charisma).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 2,
                        Max = 7,
                    });
                b.OwnsOne(e => e.CriticalHitChance).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 1d,
                        Max = 3d,
                    });
                b.OwnsOne(e => e.Damage).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 2,
                        Max = 9,
                    });
                b.OwnsOne(e => e.DodgeRate).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 37,
                        Max = 43,
                    });
                b.OwnsOne(e => e.Level).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 1,
                        Max = 3,
                    });
                b.OwnsOne(e => e.ParryChance).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 1d,
                        Max = 2d,
                    });
                b.OwnsOne(e => e.Power).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 2,
                        Max = 9,
                    });
                b.OwnsOne(e => e.DcxLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 1.23,
                        Max = 1.83,
                    });
                b.OwnsOne(e => e.GoldLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 1,
                        Min = 1,
                        Max = 2,
                    });
            });

            builder.Entity<MonsterTemplateOld>(b =>
            {
                b.HasData(new MonsterTemplateOld()
                {
                    Id = 2,
                    Name = "Rival Adventurer",
                    Description = "Ah, a fellow adventurer. Those runes make it look like he has seen a thing or two, and you think you've seen him in town before.",
                    AppearanceChance = 20,
                    SpecialAbilityCastChance = 15,
                    ImageFilePath = @"\images\monsters\rival_adventurer.jpg",
                    MonsterClass = CharacterClass.Ranger,
                    WeaponDescription = "Runed Axe",
                    ArmorDescription = "Chain Shirt",
                    CharismaOpportunityChance = 40,
                    CombatOpportunityChance = 30,
                    DcxLootChance = 20,
                    TileId = 1,
                });                
                b.OwnsOne(e => e.MitigationMelee).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 10d,
                        Max = 20d,
                    });
                b.OwnsOne(e => e.MitigationRange).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 10d,
                        Max = 20d,
                    });
                b.OwnsOne(e => e.MitigationMagic).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 0d,
                        Max = 0d,
                    });
                b.OwnsOne(e => e.MaxHitPoints).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 40,
                        Max = 50,
                    });
                b.OwnsOne(e => e.HitRate).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 38,
                        Max = 45,
                    });
                b.OwnsOne(e => e.Charisma).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 5,
                        Max = 9,
                    });
                b.OwnsOne(e => e.CriticalHitChance).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 2d,
                        Max = 5d,
                    });
                b.OwnsOne(e => e.Quickness).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 2,
                        Max = 12,
                    });
                b.OwnsOne(e => e.Damage).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 2,
                        Max = 6,
                    });
                b.OwnsOne(e => e.DodgeRate).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 10,
                        Max = 22,
                    });
                b.OwnsOne(e => e.Level).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 1,
                        Max = 4,
                    });
                b.OwnsOne(e => e.ParryChance).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 5d,
                        Max = 10d,
                    });
                b.OwnsOne(e => e.Power).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 3,
                        Max = 7,
                    });
                b.OwnsOne(e => e.DcxLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 1.33,
                        Max = 1.93,
                    });
                b.OwnsOne(e => e.GoldLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 2,
                        Min = 1,
                        Max = 2,
                    });
            });

            builder.Entity<MonsterTemplateOld>(b =>
            {
                b.HasData(new MonsterTemplateOld()
                {
                    Id = 3,
                    Name = "Giant Wolf",
                    Description = "It's fearsome. It's gray. It's big, and it's furry. Yep, that's a giant wolf.",
                    AppearanceChance = 15,
                    SpecialAbilityCastChance = 10,
                    ImageFilePath = @"\images\monsters\giant_wolf.jpg",
                    MonsterClass = CharacterClass.Warrior,
                    WeaponDescription = "something",
                    ArmorDescription = "something",
                    CharismaOpportunityChance = 25,
                    CombatOpportunityChance = 15,
                    DcxLootChance = 22,
                    TileId = 1,
                });
                b.OwnsOne(e => e.MitigationMelee).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MitigationRange).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MitigationMagic).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MaxHitPoints).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 40,
                        Max = 60,
                    });
                b.OwnsOne(e => e.HitRate).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 38,
                        Max = 45,
                    });
                b.OwnsOne(e => e.Quickness).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 2,
                        Max = 12,
                    });
                b.OwnsOne(e => e.Charisma).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 6,
                        Max = 12,
                    });
                b.OwnsOne(e => e.CriticalHitChance).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 4d,
                        Max = 8d,
                    });
                b.OwnsOne(e => e.Damage).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 2,
                        Max = 7,
                    });
                b.OwnsOne(e => e.DodgeRate).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 30,
                        Max = 40,
                    });
                b.OwnsOne(e => e.Level).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 1,
                        Max = 4,
                    });
                b.OwnsOne(e => e.ParryChance).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 4d,
                        Max = 8d,
                    });
                b.OwnsOne(e => e.Power).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 4,
                        Max = 5,
                    });
                b.OwnsOne(e => e.DcxLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 1.43,
                        Max = 1.86,
                    });
                b.OwnsOne(e => e.GoldLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 3,
                        Min = 2,
                        Max = 3,
                    });
            });

            builder.Entity<MonsterTemplateOld>(b =>
            {
                b.HasData(new MonsterTemplateOld()
                {
                    Id = 4,
                    Name = "Mighty Stag",
                    Description = "A mighty stag stands before you, and you feel the absurd urge to kneel before such a princely animal.",
                    AppearanceChance = 15,
                    SpecialAbilityCastChance = 30,
                    ImageFilePath = @"\images\monsters\mighty_stag.jpg",
                    MonsterClass = CharacterClass.Warrior,
                    WeaponDescription = "Antlers",
                    ArmorDescription = "Rough Hide",
                    CharismaOpportunityChance = 25,
                    CombatOpportunityChance = 15,
                    DcxLootChance = 50,
                    TileId = 1,
                });
                b.OwnsOne(e => e.MitigationMelee).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MitigationRange).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MitigationMagic).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 0d,
                        Max = 15d,
                    });
                b.OwnsOne(e => e.MaxHitPoints).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 40,
                        Max = 60,
                    });
                b.OwnsOne(e => e.HitRate).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 38,
                        Max = 45,
                    });
                b.OwnsOne(e => e.Quickness).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 2,
                        Max = 12,
                    });
                b.OwnsOne(e => e.Charisma).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 6,
                        Max = 12,
                    });
                b.OwnsOne(e => e.CriticalHitChance).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 4d,
                        Max = 8d,
                    });
                b.OwnsOne(e => e.Damage).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 2,
                        Max = 7,
                    });
                b.OwnsOne(e => e.DodgeRate).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 30,
                        Max = 40,
                    });
                b.OwnsOne(e => e.Level).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 2,
                        Max = 4,
                    });
                b.OwnsOne(e => e.ParryChance).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 4d,
                        Max = 8d,
                    });
                b.OwnsOne(e => e.Power).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 4,
                        Max = 5,
                    });
                b.OwnsOne(e => e.DcxLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 1.53,
                        Max = 2.22,
                    });
                b.OwnsOne(e => e.GoldLootAmount).HasData(
                    new
                    {
                        MonsterTemplateId = 4,
                        Min = 2,
                        Max = 4,
                    });
            });
        }
    }
}
