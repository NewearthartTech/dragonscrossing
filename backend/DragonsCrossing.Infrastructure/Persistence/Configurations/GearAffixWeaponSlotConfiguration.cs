using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class GearAffixWeaponSlotConfiguration : IEntityTypeConfiguration<GearAffixWeaponSlot>
    {
        public void Configure(EntityTypeBuilder<GearAffixWeaponSlot> builder)
        {
            builder.ToTable(nameof(GearAffixWeaponSlot));
            
            // convert enums to strings
            builder.Property(e => e.WeaponSlotType)
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
