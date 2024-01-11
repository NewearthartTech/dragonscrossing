using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Heroes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class ArmorTemplateSeed
    {
        public ArmorTemplateSeed(ModelBuilder builder)
        {
            builder.Entity<ArmorTemplate>(b =>
            {
                // Basic Leather armor - all races
                b.HasData(new ArmorTemplate()
                {
                    Id = 1,
                    Name = "Basic Leather Armor",
                    Description = "Standard body armor for defending",                    
                    HeroClass = null,
                    IsStartingGear = true,
                    PurchasePrice = 1,
                    SellPrice = 1,
                    ArmorType = ArmorType.Leathers,
                    ImageBaseUrl = "/images/items/weapons/BasicLeatherArmor.jpg",
                    HeroStatModifiers = null,
                    SlotType = ArmorSlotType.Chest,                                        
                });
                b.OwnsOne(e => e.Defense).HasData(
                    new
                    {
                        ArmorTemplateId = 1,
                        Min = 2.75d,
                        Max = 2.75d,
                    });
                b.OwnsOne(e => e.CriticalHit).HasData(
                    new
                    {
                        ArmorTemplateId = 1,
                        Min = .5d, // TODO: this armor doesn't have crit hit bonus, so remove this when done testing.
                        Max = 1d,
                    });
            });
        }
    }
}
