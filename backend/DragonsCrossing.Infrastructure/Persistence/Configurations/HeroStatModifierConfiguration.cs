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
    public class HeroStatModifierConfiguration : IEntityTypeConfiguration<HeroStatModifier>
    {
        public void Configure(EntityTypeBuilder<HeroStatModifier> builder)
        {
            builder.OwnsOne(b => b.EffectAmountOffsetRange);
        }
    }
}
