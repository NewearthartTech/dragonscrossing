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
    public class HeroSkillTemplateConfiguration : IEntityTypeConfiguration<HeroSkillTemplate>
    {
        public void Configure(EntityTypeBuilder<HeroSkillTemplate> builder)
        {
            builder.ToTable(nameof(HeroSkillTemplate));

            // convert enums to strings
            builder.Property(e => e.HeroClass)
                .HasConversion<string>()
                .HasMaxLength(25);

            builder.HasMany(e => e.Zones)
                .WithOne()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
