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
    public class HeroSkillConfiguration : IEntityTypeConfiguration<HeroSkill>
    {
        public void Configure(EntityTypeBuilder<HeroSkill> builder)
        {
            builder.ToTable(nameof(HeroSkill));
            
            builder.HasOne(e => e.SkillTemplate)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            // convert enums to strings
            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
