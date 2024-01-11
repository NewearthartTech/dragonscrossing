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
    public class GearAffixTemplateConfiguration : IEntityTypeConfiguration<GearAffixTemplate>
    {
        public void Configure(EntityTypeBuilder<GearAffixTemplate> builder)
        {
            builder.ToTable(nameof(GearAffixTemplate));

            builder.Navigation(b => b.WeaponSlots).AutoInclude();
            builder.Navigation(b => b.ArmorSlots).AutoInclude();
            builder.Navigation(b => b.Tiers).AutoInclude();

            // convert enums to strings
            builder.Property(e => e.Effect)
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
