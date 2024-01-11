using DragonsCrossing.Domain.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable(nameof(Player));

            builder.HasIndex(b => b.BlockchainPublicAddress)                
                .IsUnique();

            builder.Property(e => e.BlockchainPublicAddress)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.Name)
                .HasMaxLength(64);

            builder.Property(e => e.SignedSignature)
                .HasMaxLength(64);
        }
    }
}
