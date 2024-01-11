using DragonsCrossing.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class ZoneSeed
    {
        public ZoneSeed(ModelBuilder builder)
        {
            builder.Entity<Zone>(b =>
            {
                b.HasData(new Zone()
                {
                    Id = 1,
                    Name = "Aedos",
                    LoreEncountersRequired = 0,
                    Order = 1,
                });
                b.HasData(new Zone()
                {
                    Id = 2,
                    Name = "Wild Prairie",
                    LoreEncountersRequired = 2,
                    Order = 2,                                        
                });
                b.HasData(new Zone()
                {
                    Id = 3,
                    Name = "Mysterious Forest",
                    LoreEncountersRequired = 3,
                    Order = 3,
                });
                b.HasData(new Zone()
                {
                    Id = 4,
                    Name = "Foul Wastes",
                    LoreEncountersRequired = 4,
                    Order = 4,
                });
                b.HasData(new Zone()
                {
                    Id = 5,
                    Name = "Treacherous Peaks",
                    LoreEncountersRequired = 5,
                    Order = 5,
                });
                b.HasData(new Zone()
                {
                    Id = 6,
                    Name = "Dark Tower",
                    LoreEncountersRequired = 6,
                    Order = 6,
                });
            });                          
        }
    }
}
