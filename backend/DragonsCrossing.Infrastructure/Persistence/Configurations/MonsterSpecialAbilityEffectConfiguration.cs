using DragonsCrossing.Domain.Monsters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class MonsterSpecialAbilityEffectConfiguration : IEntityTypeConfiguration<MonsterSpecialAbilityEffect>
    {
        public void Configure(EntityTypeBuilder<MonsterSpecialAbilityEffect> builder)
        {
            // enums to strings
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(50);
            builder.Property(e => e.EffectWho)
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
