using DragonsCrossing.Domain.GameStates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    internal class GameStateConfiguration : IEntityTypeConfiguration<GameState>
    {
        public void Configure(EntityTypeBuilder<GameState> builder)
        {
            builder.ToTable(nameof(GameState));
            builder.Navigation(b => b.Combat).AutoInclude();
            builder.Navigation(b => b.CurrentZone).AutoInclude();
            //builder.Navigation(b => b.Hero).AutoInclude();
        }
    }
}
