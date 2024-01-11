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
    public class CombatDetailConfiguration : IEntityTypeConfiguration<CombatDetail>
    {
        public void Configure(EntityTypeBuilder<CombatDetail> builder)
        {
            builder.ToTable(nameof(CombatDetail));

            // convert enums to strings
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(40);
        }
    }
}
