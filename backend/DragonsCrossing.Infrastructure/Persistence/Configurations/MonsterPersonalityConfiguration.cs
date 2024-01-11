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
    public class MonsterPersonalityConfiguration : IEntityTypeConfiguration<MonsterPersonality>
    {
        public void Configure(EntityTypeBuilder<MonsterPersonality> builder)
        {
            builder.ToTable(nameof(MonsterPersonality));
            // convert enums to strings
            builder.Property(e => e.PersonalityType)
                .HasConversion<string>()
                .HasMaxLength(30);
        }
    }
}
