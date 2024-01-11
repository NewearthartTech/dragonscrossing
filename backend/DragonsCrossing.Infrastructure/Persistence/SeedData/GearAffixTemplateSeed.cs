using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class GearAffixTemplateSeed
    {
        public GearAffixTemplateSeed(ModelBuilder builder)
        {
            builder.Entity<GearAffixTemplate>(b =>
            {
                // weapon affixes
                b.HasData(new GearAffixTemplate()
                {
                    Id = 1,
                    Effect = GearAffixEffect.Draining,
                    EffectDescription = "Integer Lifesteal",
                    
                    
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 2,
                    Effect = GearAffixEffect.Truthful,
                    EffectDescription = "Increases chance to hit",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 3,
                    Effect = GearAffixEffect.Disarming,
                    EffectDescription = "Chance to have monster do 50% reduced damage for next attack.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 4,
                    Effect = GearAffixEffect.Savage,
                    EffectDescription = "Deal increased integer damage on subsequent hits.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 5,
                    Effect = GearAffixEffect.Stunning,
                    EffectDescription = "Chance to have monster unable to use special attack next turn and has 50% reduced dodge next turn as well",
                });

                // armor affixes
                b.HasData(new GearAffixTemplate()
                {
                    Id = 6,
                    Effect = GearAffixEffect.Quick,
                    EffectDescription = "Increase to dodge.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 7,
                    Effect = GearAffixEffect.Thick,
                    EffectDescription = "Increase to armor mitigation.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 8,
                    Effect = GearAffixEffect.Reflective,
                    EffectDescription = "Integer return of damage when hit.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 9,
                    Effect = GearAffixEffect.Recovering,
                    EffectDescription = "Recovers HP after battle is completed.",
                });

                // Ring
                b.HasData(new GearAffixTemplate()
                {
                    Id = 10,
                    Effect = GearAffixEffect.Deft,
                    EffectDescription = "Increased parry %",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 11,
                    Effect = GearAffixEffect.Doubling,
                    EffectDescription = "Increased % chance to swing twice.",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 12,
                    Effect = GearAffixEffect.Fetching,
                    EffectDescription = "Increased Charisma",
                });

                // every gear slot but secondary weapon
                b.HasData(new GearAffixTemplate()
                {
                    Id = 13,
                    Effect = GearAffixEffect.Strong,
                    EffectDescription = "Increased Strength",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 14,
                    Effect = GearAffixEffect.Agile,
                    EffectDescription = "Increased Agility",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 15,
                    Effect = GearAffixEffect.Wise,
                    EffectDescription = "Increased Wisdom",
                });
                b.HasData(new GearAffixTemplate()
                {
                    Id = 16,
                    Effect = GearAffixEffect.Vital,
                    EffectDescription = "Increased HP",
                });
            });
        }
    }
}
