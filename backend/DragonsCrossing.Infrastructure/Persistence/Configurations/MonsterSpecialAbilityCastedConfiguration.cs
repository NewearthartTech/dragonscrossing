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
    public class MonsterSpecialAbilityCastedConfiguration : IEntityTypeConfiguration<MonsterSpecialAbilityCasted>
    {
        public void Configure(EntityTypeBuilder<MonsterSpecialAbilityCasted> builder)
        {
            builder.ToTable(nameof(MonsterSpecialAbilityCasted));
            // disable cascade delete since we don't ever want to delete an effect if the MonsterSpecialAbilityCasted is deleted. EF will throw error if this isn't disabled.
            builder.HasOne(e => e.Effect)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
