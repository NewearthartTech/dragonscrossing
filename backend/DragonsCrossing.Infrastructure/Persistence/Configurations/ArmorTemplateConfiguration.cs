using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class ArmorTemplateConfiguration : IEntityTypeConfiguration<ArmorTemplate>
    {
        public void Configure(EntityTypeBuilder<ArmorTemplate> builder)
        {
            builder.ToTable(nameof(ArmorTemplate));
            
            builder.OwnsOne(r => r.Defense);
            builder.OwnsOne(r => r.CriticalHit);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(e => e.Description)
                .HasMaxLength(250);
            builder.Property(e => e.ImageBaseUrl)
                .IsRequired()
                .HasMaxLength(250);            

            // convert enums to strings
            builder.Property(e => e.HeroClass)
                .HasConversion<string>()
                .HasMaxLength(25);
            builder.Property(e => e.SlotType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(15);
            builder.Property(e => e.ArmorType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);                        
        }
    }
}
