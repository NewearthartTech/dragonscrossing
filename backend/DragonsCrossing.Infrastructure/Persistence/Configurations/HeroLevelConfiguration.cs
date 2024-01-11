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
    public class HeroLevelConfiguration : IEntityTypeConfiguration<HeroLevel>
    {
        public void Configure(EntityTypeBuilder<HeroLevel> builder)
        {
            builder.ToTable(nameof(HeroLevel));
            builder.HasIndex(b => b.Number)
                .IsUnique();            
        }
    }
}
