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
    public class HeroSkillCastedConfiguration : IEntityTypeConfiguration<HeroSkillCasted>
    {
        public void Configure(EntityTypeBuilder<HeroSkillCasted> builder)
        {
            builder.ToTable(nameof(HeroSkillCasted));
            builder.HasOne(e => e.SkillCasted)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
