using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Weapons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class WeaponTemplateSeed
    {
        public WeaponTemplateSeed(ModelBuilder builder)
        {
            // weapon templates
            builder.Entity<WeaponTemplate>(b =>
            {
                // Basic Greatsword - warrior
                b.HasData(new WeaponTemplate()
                {
                    Id = 1,
                    Name = "Basic Greatsword",
                    Description = "A standard sword for cutting foes",
                    HeroClass = CharacterClass.Warrior,
                    IsStartingGear = true,
                    PurchasePrice = 1,
                    SellPrice = 1,  
                    CriticalHit = null,
                    DoubleHit = null,
                    Dodge = null,
                    WeaponType = WeaponType.Greatsword,
                    ImageBaseUrl = "/img/api/items/BasicGreatsword.jpg",
                    HeroStatModifiers = null,
                    SlotType = WeaponSlotType.TwoHand,
                });
                b.OwnsOne(e => e.BaseDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 1,
                        Min = 1,
                        Max = 10,
                    });
                b.OwnsOne(e => e.BonusDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 1,
                        Min = -2d,
                        Max = 1d,
                    });
                b.OwnsOne(e => e.Parry).HasData(
                    new
                    {
                        WeaponTemplateId = 1,
                        Min = 2d,
                        Max = 4d,
                    });

                // Basic Crossbow - ranger
                b.HasData(new WeaponTemplate()
                {
                    Id = 2,
                    Name = "Basic Crossbow",
                    Description = "A standard crossbow for poking holes into foes",
                    HeroClass = CharacterClass.Ranger,
                    IsStartingGear = true,
                    PurchasePrice = 1,
                    SellPrice = 1,
                    CriticalHit = null,
                    DoubleHit = null,
                    Dodge = null,
                    WeaponType = WeaponType.Crossbow,
                    ImageBaseUrl = "/images/items/weapons/BasicCrossbow.jpg",
                    HeroStatModifiers = null,
                    SlotType = WeaponSlotType.TwoHand,
                });
                b.OwnsOne(e => e.BaseDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 2,
                        Min = 1,
                        Max = 10,
                    });
                b.OwnsOne(e => e.BonusDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 2,
                        Min = -2d,
                        Max = 1d,
                    });
                b.OwnsOne(e => e.Parry).HasData(
                    new
                    {
                        WeaponTemplateId = 2,
                        Min = 2d,
                        Max = 4d,
                    });

                // Basic staff - mage
                b.HasData(new WeaponTemplate()
                {
                    Id = 3,
                    Name = "Basic Staff",
                    Description = "A standard staff for casting magic",
                    HeroClass = CharacterClass.Mage,
                    IsStartingGear = true,
                    PurchasePrice = 1,
                    SellPrice = 1,
                    CriticalHit = null,
                    DoubleHit = null,
                    Dodge = null,
                    WeaponType = WeaponType.Staff,
                    ImageBaseUrl = "/images/items/weapons/BasicStaff.jpg",
                    HeroStatModifiers = null,
                    SlotType = WeaponSlotType.TwoHand,
                });
                b.OwnsOne(e => e.BaseDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 3,
                        Min = 1,
                        Max = 8,
                    });
                b.OwnsOne(e => e.BonusDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 3,
                        Min = -2d,
                        Max = 1d,
                    });
                b.OwnsOne(e => e.Parry).HasData(
                    new
                    {
                        WeaponTemplateId = 3,
                        Min = 2d,
                        Max = 4d,
                    });

                // Simple Shortsword
                b.HasData(new WeaponTemplate()
                {
                    Id = 4,
                    Name = "Simple Shortsword",
                    Description = "Stabs things",
                    HeroClass = CharacterClass.Ranger,
                    IsStartingGear = false,
                    PurchasePrice = 1,
                    SellPrice = 1,
                    CriticalHit = null,
                    DoubleHit = null,
                    Dodge = null,
                    WeaponType = WeaponType.Staff,
                    ImageBaseUrl = "/images/items/weapons/simple-shortsword",
                    HeroStatModifiers = null,
                    SlotType = WeaponSlotType.PrimaryHand,
                });
                b.OwnsOne(e => e.BaseDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 4,
                        Min = 1,
                        Max = 6,
                    });
                b.OwnsOne(e => e.BonusDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 4,
                        Min = -1d,
                        Max = 2d,
                    });
                b.OwnsOne(e => e.Parry).HasData(
                    new
                    {
                        WeaponTemplateId = 4,
                        Min = 2d,
                        Max = 4d,
                    });

                // Runed Axe
                b.HasData(new WeaponTemplate()
                {
                    Id = 5,
                    Name = "Runed Axe",
                    Description = "chops things",
                    HeroClass = CharacterClass.Warrior,
                    IsStartingGear = false,
                    PurchasePrice = 1,
                    SellPrice = 1,
                    CriticalHit = null,
                    DoubleHit = null,
                    Dodge = null,
                    WeaponType = WeaponType.Handaxe,
                    ImageBaseUrl = "/images/items/weapons/runed-axe",
                    HeroStatModifiers = null,
                    SlotType = WeaponSlotType.PrimaryHand,                    
                });
                b.OwnsOne(e => e.BaseDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 5,
                        Min = 1,
                        Max = 6,
                    });
                b.OwnsOne(e => e.BonusDamage).HasData(
                    new
                    {
                        WeaponTemplateId = 5,
                        Min = -1d,
                        Max = 3d,
                    });
                b.OwnsOne(e => e.Parry).HasData(
                    new
                    {
                        WeaponTemplateId = 5,
                        Min = 2d,
                        Max = 4d,
                    });
            });
        }        
    }
}
