using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class NftItemTemplateConfiguration : IEntityTypeConfiguration<NftItemTemplate>
    {
        public void Configure(EntityTypeBuilder<NftItemTemplate> builder)
        {
            builder.ToTable(nameof(NftItemTemplate));
            builder.Property(e => e.Name)
                .HasMaxLength(20);
            builder.Property(e => e.Description)
                .HasMaxLength(100);
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(30);
            builder.Property(e => e.ImageBaseUrl)
                .HasMaxLength(250);
            builder.Property(e => e.DcxCostToOpen)
                .HasPrecision(12, 9);
        }
    }
}
