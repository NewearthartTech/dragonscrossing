using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class ArmorAffixConfiguration : IEntityTypeConfiguration<ArmorAffix>
    {
        public void Configure(EntityTypeBuilder<ArmorAffix> builder)
        {
            builder.ToTable(nameof(ArmorAffix));

            // disable cascade delete since we don't ever want to delete a tile if a DiscoveredTile is deleted
            builder.HasOne(e => e.Tier)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
