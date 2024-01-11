using DragonsCrossing.Domain.Heroes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class HeroNameConfiguration : IEntityTypeConfiguration<HeroName>
    {
        public void Configure(EntityTypeBuilder<HeroName> builder)
        {
            builder.ToTable(nameof(HeroName));

            // disable cascade delete since we don't ever want to delete a hero if a DiscoveredTile is deleted
            //builder.HasOne(e => e.Hero)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
