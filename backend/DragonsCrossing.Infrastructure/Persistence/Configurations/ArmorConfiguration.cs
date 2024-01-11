using DragonsCrossing.Domain.Armors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class ArmorConfiguration : IEntityTypeConfiguration<Armor>
    {
        public void Configure(EntityTypeBuilder<Armor> builder)
        {
            builder.ToTable("Armor");
            builder.Navigation(b => b.ArmorTemplate).AutoInclude();

            // convert enums to strings
            builder.Property(e => e.Rarity)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(15);
        }
    }
}
