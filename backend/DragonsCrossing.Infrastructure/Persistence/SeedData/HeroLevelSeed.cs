using DragonsCrossing.Domain.Heroes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class HeroLevelSeed
    {
        public HeroLevelSeed(ModelBuilder builder)
        {
            builder.Entity<HeroLevel>().HasData(
                new HeroLevel()
                {
                    Id = 1,
                    Number = 1,
                    MaxExperiencePoints = 20,
                    AdditionalQuests = 0,
                },
                new HeroLevel()
                {
                    Id = 2,
                    Number = 2,
                    MaxExperiencePoints = 41,
                    AdditionalQuests = 0,
                },
                new HeroLevel()
                {
                    Id = 3,
                    Number = 3,
                    MaxExperiencePoints = 63,
                    AdditionalQuests = 1,
                },
                new HeroLevel()
                {
                    Id = 4,
                    Number = 4,
                    MaxExperiencePoints = 86,
                    AdditionalQuests = 1,
                }
            );
        }
    }
}
