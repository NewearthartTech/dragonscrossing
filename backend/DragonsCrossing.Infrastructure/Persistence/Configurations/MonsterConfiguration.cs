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
    public class MonsterConfiguration : IEntityTypeConfiguration<Monster>
    {
        public void Configure(EntityTypeBuilder<Monster> builder)
        {
            builder.ToTable(nameof(Monster));
            builder.Navigation(b => b.SpecialAbilitiesCasted).AutoInclude();
            builder.Navigation(b => b.MonsterTemplateOld).AutoInclude();
            builder.Navigation(b => b.Personality).AutoInclude();
        }
    }
}
