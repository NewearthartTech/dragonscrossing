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
    public class HeroInventoryConfiguration : IEntityTypeConfiguration<HeroInventory>
    {
        public void Configure(EntityTypeBuilder<HeroInventory> builder)
        {
            builder.ToTable(nameof(HeroInventory));         

            // these are just read only helper properties so don't store them in the db
            builder.Ignore(b => b.EquippedWeapons);
            builder.Ignore(b => b.EquippedArmors);

            // always include the following child classes
            builder.Navigation(b => b.Weapons).AutoInclude();
            builder.Navigation(b => b.Armors).AutoInclude();
            builder.Navigation(b => b.NftItems).AutoInclude();
        }
    }
}
