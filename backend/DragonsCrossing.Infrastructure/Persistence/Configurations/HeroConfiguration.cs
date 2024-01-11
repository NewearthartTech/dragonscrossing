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
    public class HeroConfiguration : IEntityTypeConfiguration<Hero>
    {
        public void Configure(EntityTypeBuilder<Hero> builder)
        {
            builder.ToTable(nameof(Hero));
            //builder.HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);                        
            builder.Property(d => d.ImageBaseUrl)
                .IsRequired()
                .HasMaxLength(100);

            // convert enums to strings
            builder.Property(e => e.HeroClass)
                .HasConversion<string>()
                .HasMaxLength(25);
            builder.Property(e => e.Rarity)
                .HasConversion<string>()
                .HasMaxLength(50); 
            builder.Property(e => e.Gender)
                .HasConversion<string>()
                .HasMaxLength(10);

            // automatically load the hero name and template every time a hero is returned
            // NOTE: I may not want to load the hero template every time.
            builder.Navigation(b => b.HeroName).AutoInclude();
            builder.Navigation(b => b.HeroTemplate).AutoInclude();
            builder.Navigation(b => b.Level).AutoInclude();

            // disable cascade delete since we don't ever want to delete a hero if a DiscoveredTile is deleted
            builder.HasOne(e => e.HeroName)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.Level)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.HeroTemplate)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);


            //builder.Property(d => d.CreatedBy)                
            //    .HasMaxLength(100);
            //builder.Property(d => d.ModifiedBy)                
            //    .HasMaxLength(100);
        }
    }
}
