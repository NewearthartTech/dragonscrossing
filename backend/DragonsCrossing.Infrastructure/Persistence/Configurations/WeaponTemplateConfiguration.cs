using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Weapons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class WeaponTemplateConfiguration : IEntityTypeConfiguration<WeaponTemplate>
    {
        public void Configure(EntityTypeBuilder<WeaponTemplate> builder)
        {
            builder.ToTable(nameof(WeaponTemplate));
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(w => w.Description)
                .HasMaxLength(250);
            builder.Property(e => e.ImageBaseUrl)
                .IsRequired()
                .HasMaxLength(250);

            builder.OwnsOne(r => r.BonusDamage);
            builder.OwnsOne(r => r.CriticalHit);
            builder.OwnsOne(r => r.DoubleHit);
            builder.OwnsOne(r => r.Dodge);
            builder.OwnsOne(r => r.Parry);
            builder.OwnsOne(r => r.BaseDamage);

            // convert enums to strings
            builder.Property(e => e.HeroClass)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(25);
            builder.Property(e => e.SlotType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(15);
            builder.Property(e => e.WeaponType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);
            builder.Property(e => e.SlotType)
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
