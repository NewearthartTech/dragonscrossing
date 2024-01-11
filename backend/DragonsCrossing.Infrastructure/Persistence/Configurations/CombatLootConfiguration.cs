using DragonsCrossing.Domain.Combats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class CombatLootConfiguration : IEntityTypeConfiguration<CombatLoot>
    {
        public void Configure(EntityTypeBuilder<CombatLoot> builder)
        {
            builder.ToTable(nameof(CombatLoot));
            builder.Property(b => b.UnsecuredDcx).HasPrecision(12, 9);
        }
    }
}
