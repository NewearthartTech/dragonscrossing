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
    internal class HeroTemplateConfiguration : IEntityTypeConfiguration<HeroTemplate>
    {
        public void Configure(EntityTypeBuilder<HeroTemplate> builder)
        {
            builder.ToTable(nameof(HeroTemplate));
            builder.OwnsOne(r => r.StartingStatPointsEachStat);
            builder.OwnsOne(r => r.NoWeaponDamage);

            builder.Property(d => d.ImageBaseUrl)
                .IsRequired()
                .HasMaxLength(75);
        }
    }
}
