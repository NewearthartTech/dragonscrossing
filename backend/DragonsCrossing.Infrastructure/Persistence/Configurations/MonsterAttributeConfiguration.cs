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
    public class MonsterAttributeConfiguration : IEntityTypeConfiguration<MonsterAttribute>
    {
        public void Configure(EntityTypeBuilder<MonsterAttribute> builder)
        {
            builder.ToTable(nameof(MonsterAttribute));
            
            // convert enums to strings
            builder.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(30);
        }
    }
}
