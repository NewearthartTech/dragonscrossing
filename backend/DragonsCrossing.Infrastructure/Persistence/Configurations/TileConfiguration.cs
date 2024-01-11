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
    public class TileConfiguration : IEntityTypeConfiguration<Tile>
    {
        public void Configure(EntityTypeBuilder<Tile> builder)
        {
            builder.ToTable(nameof(Tile));
            builder.OwnsOne(m => m.HeroLevelRequired);
            builder.Property(x => x.Name)
                .HasMaxLength(50);

            // enums to strings
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(30);
        }
    }
}
