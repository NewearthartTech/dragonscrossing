using DragonsCrossing.Domain.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.ToTable(nameof(Zone));
            builder.Property(x => x.Name)
                .HasMaxLength(50);

            builder.HasMany(e => e.HeroSkillsAllowed)
                .WithOne()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
