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
    public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
    {
        public void Configure(EntityTypeBuilder<Weapon> builder)
        {
            builder.ToTable(nameof(Weapon));
            builder.Navigation(b => b.WeaponTemplate).AutoInclude();

            // convert enums to strings
            builder.Property(e => e.Rarity)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
