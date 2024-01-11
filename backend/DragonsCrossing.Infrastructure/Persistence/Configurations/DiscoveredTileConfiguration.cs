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
    public class DiscoveredTileConfiguration : IEntityTypeConfiguration<DiscoveredTile>
    {
        public void Configure(EntityTypeBuilder<DiscoveredTile> builder)
        {
            builder.ToTable(nameof(DiscoveredTile));

            // disable cascade delete since we don't ever want to delete a hero if a DiscoveredTile is deleted
            //builder.HasOne(e => e.Hero)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.NoAction);

            // disable cascade delete since we don't ever want to delete a tile if a DiscoveredTile is deleted
            builder.HasOne(e => e.Tile)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            // create a unique constraint on the two fields
            builder.HasIndex(e => new { e.HeroId, e.TileId }).IsUnique();
        }
    }
}
