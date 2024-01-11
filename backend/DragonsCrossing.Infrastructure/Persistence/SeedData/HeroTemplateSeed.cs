using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class HeroTemplateSeed
    {
        public HeroTemplateSeed(ModelBuilder builder)
        {
            builder.Entity<HeroTemplate>(b =>
            {
                b.HasData(
                new HeroTemplate()
                {
                    Id = 1,
                    ImageBaseUrl = @"/img/api/heroes/",
                    MaxHitPoints = 25,
                    TotalSkillPoints = 20,
                    DateCreated = DateTime.Now,
                    CreatedBy = nameof(HeroTemplateSeed),
                    TotalDailyQuests = 20,
                    IsActive = true,
                });
                // See this for more info: https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
                b.OwnsOne(e => e.StartingStatPointsEachStat).HasData(
                    new // pretend this anonymous class is RangeInt
                    { 
                        HeroTemplateId = 1,
                        Min = 4,
                        Max = 10,
                    });
                b.OwnsOne(e => e.NoWeaponDamage).HasData(
                    new // pretend this anonymous class is RangeInt
                    {
                        HeroTemplateId = 1,
                        Min = 1d,
                        Max = 4d,
                    });
            });        
        }
    }
}
