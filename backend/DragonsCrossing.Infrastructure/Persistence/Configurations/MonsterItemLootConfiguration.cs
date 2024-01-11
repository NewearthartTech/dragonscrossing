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
    public class MonsterItemLootConfiguration : IEntityTypeConfiguration<MonsterItemLoot>
    {
        public void Configure(EntityTypeBuilder<MonsterItemLoot> builder)
        {
            builder.ToTable(nameof(MonsterItemLoot));

            // auto include properties
            builder.Navigation(b => b.WeaponTemplate).AutoInclude();
            builder.Navigation(b => b.ArmorTemplate).AutoInclude();
            builder.Navigation(b => b.NftItemTemplate).AutoInclude();
        }
    }
}
