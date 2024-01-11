using DragonsCrossing.Domain.Heroes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class HeroNameSeed
    {
        public HeroNameSeed(ModelBuilder builder)
        {
            builder.Entity<HeroName>().HasData(
                new HeroName()
                {
                    Id = 1,
                    Name = "Legolas",                    
                },
                new HeroName()
                {
                    Id = 2,
                    Name = "Aragon",
                },
                new HeroName()
                {
                    Id = 3,
                    Name = "Gandalf",
                },
                new HeroName()
                {
                    Id = 4,
                    Name = "Harry Potter",
                },
                new HeroName()
                {
                    Id = 5,
                    Name = "Snape",
                },
                new HeroName()
                {
                    Id = 6,
                    Name = "Dumbeldor",
                },
                new HeroName()
                {
                    Id = 7,
                    Name = "Voldermort",
                },
                new HeroName()
                {
                    Id = 8,
                    Name = "Sauron",
                },
                new HeroName()
                {
                    Id = 9,
                    Name = "Gimly",
                },
                new HeroName()
                {
                    Id = 10,
                    Name = "Samwise",
                });
        }
    }
}
