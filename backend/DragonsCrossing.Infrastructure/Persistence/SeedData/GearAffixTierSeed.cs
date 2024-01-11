using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class GearAffixTierSeed
    {
        public GearAffixTierSeed(ModelBuilder builder)
        {
            builder.Entity<GearAffixTier>(b =>
            {
                // Draining Zone 1
                b.HasData(new GearAffixTier()
                {
                    Id = 1,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 1,
                    GearAffixTemplateId = 1,
                    Amount = 1,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 2,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 1,
                    GearAffixTemplateId = 1,
                    Amount = 1,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 3,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 1,
                    GearAffixTemplateId = 1,
                    Amount = 2,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 4,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 1,
                    GearAffixTemplateId = 1,
                    Amount = 2,
                });

                // Draining Zone 2
                b.HasData(new GearAffixTier()
                {
                    Id = 5,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 2,
                    GearAffixTemplateId = 1,
                    Amount = 2,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 6,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 2,
                    GearAffixTemplateId = 1,
                    Amount = 2,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 7,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 2,
                    GearAffixTemplateId = 1,
                    Amount = 3,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 8,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 2,
                    GearAffixTemplateId = 1,
                    Amount = 4,
                });

                // Truthful Zone 1
                b.HasData(new GearAffixTier()
                {
                    Id = 9,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 1,
                    GearAffixTemplateId = 2,
                    Amount = .5,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 10,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 1,
                    GearAffixTemplateId = 2,
                    Amount = 1,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 11,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 1,
                    GearAffixTemplateId = 2,
                    Amount = 1.5,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 12,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 1,
                    GearAffixTemplateId = 2,
                    Amount = 2,
                });

                // Truthful Zone 2
                b.HasData(new GearAffixTier()
                {
                    Id = 13,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 2,
                    GearAffixTemplateId = 2,
                    Amount = .75,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 14,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 2,
                    GearAffixTemplateId = 2,
                    Amount = 1.25,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 15,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 2,
                    GearAffixTemplateId = 2,
                    Amount = 1.75,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 16,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 2,
                    GearAffixTemplateId = 2,
                    Amount = 2.25,
                });

                // Disarming Zone 1
                b.HasData(new GearAffixTier()
                {
                    Id = 17,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 1,
                    GearAffixTemplateId = 3,
                    Amount = 1,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 18,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 1,
                    GearAffixTemplateId = 3,
                    Amount = 2,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 19,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 1,
                    GearAffixTemplateId = 3,
                    Amount = 3,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 20,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 1,
                    GearAffixTemplateId = 3,
                    Amount = 4,
                });

                // Disarming Zone 2
                b.HasData(new GearAffixTier()
                {
                    Id = 21,
                    Type = GearAffixTierType.Tier1,
                    ZoneId = 2,
                    GearAffixTemplateId = 3,
                    Amount = 2,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 22,
                    Type = GearAffixTierType.Tier2,
                    ZoneId = 2,
                    GearAffixTemplateId = 3,
                    Amount = 3,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 23,
                    Type = GearAffixTierType.Tier3,
                    ZoneId = 2,
                    GearAffixTemplateId = 3,
                    Amount = 4,
                });
                b.HasData(new GearAffixTier()
                {
                    Id = 24,
                    Type = GearAffixTierType.Tier4,
                    ZoneId = 2,
                    GearAffixTemplateId = 3,
                    Amount = 5,
                });
            });
        }
    }
}
