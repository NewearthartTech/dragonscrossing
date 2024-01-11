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
    public class MonsterSpecialAbilityConfiguration : IEntityTypeConfiguration<MonsterSpecialAbility>
    {
        public void Configure(EntityTypeBuilder<MonsterSpecialAbility> builder)
        {
            builder.Property(x => x.Name)
                .HasMaxLength(50);
            builder.Property(x => x.Description)
                .HasMaxLength(256);

            // always include the following child classes
            builder.Navigation(b => b.Effects).AutoInclude();
        }
    }
}
