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
    public class CombatConfiguration : IEntityTypeConfiguration<Combat>
    {
        public void Configure(EntityTypeBuilder<Combat> builder)
        {
            builder.ToTable(nameof(Combat));
            builder.Navigation(b => b.Tile).AutoInclude();
            builder.Navigation(b => b.Monster).AutoInclude();
            builder.Navigation(b => b.Hero).AutoInclude();
            builder.Navigation(b => b.CombatDetails).AutoInclude();
        }
    }
}
